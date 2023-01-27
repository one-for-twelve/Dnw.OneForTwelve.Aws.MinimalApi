using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using Shared.JwtModels;

namespace Shared.Services;

public interface IJwtValidator
{
    Task Validate();
}

public class JwtValidator : IJwtValidator
{
    private readonly HttpContext _httpContext;
    private readonly string _authority;
    private readonly IJwtSigningKeysCache _jwtSigningKeysCache;

    public JwtValidator(HttpContext httpContext, string authority, IJwtSigningKeysCache jwtSigningKeysCache)
    {
        _httpContext = httpContext;
        _authority = authority;
        _jwtSigningKeysCache = jwtSigningKeysCache;
    }

    public async Task Validate()
    {
        var authHeader = _httpContext.Request.Headers.Authorization.ToString();
        if (authHeader == null)
        {
            throw new UnauthorizedAccessException("Authorization header missing from request");
        }
        
        var token = authHeader.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
        
        var kid = GetKidFromTokenHeader(token);

        if (!_jwtSigningKeysCache.ContainsKey(kid))
        {
            await CacheSigningKey(_authority, kid);
        }
        
        var tokenValidationResult = ValidateToken(_authority, kid, token);

        SetHttpContextUser(_httpContext, tokenValidationResult);
    }
    
    private static void SetHttpContextUser(HttpContext httpContext, TokenValidationResult tokenValidationResult)
    {
        var claims = tokenValidationResult.ClaimsIdentity.Claims;
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, null, "name", null));
        httpContext.User = user;
    }

    private TokenValidationResult ValidateToken(string authority, string kid, string token)
    {
        var tokenValidationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authority,

            ValidateAudience = true,
            ValidAudience = authority.Split('/').Last(),

            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = new[] { _jwtSigningKeysCache.GetKeyByKid(kid) },

            ValidateLifetime = true
        };
        
        var jsonWebTokenHandler = new JsonWebTokenHandler();
        var tokenValidationResult = jsonWebTokenHandler.ValidateToken(token, tokenValidationParams);

        if (tokenValidationResult.IsValid) return tokenValidationResult;
        
        // Handle each exception which tokenValidationResult can contain as appropriate for your service
        // Your service might need to respond with a http response instead of an exception.
        if (tokenValidationResult.Exception != null)
            throw tokenValidationResult.Exception;

        throw new ApplicationException("Token Validation Failed");
    }

    private static string GetKidFromTokenHeader(string token)
    {
        var headerBase64 = token.Split('.')[0];
        var headerJson = Base64Decode(headerBase64);
        var header = JsonSerializer.Deserialize<JwtHeader>(headerJson);
        
        return header!.Kid;
    }

    private async Task CacheSigningKey(string authority, string kid)
    {
        var configAddress = $"{authority}/.well-known/openid-configuration";
        var retriever = new HttpDocumentRetriever();
        var ct = CancellationToken.None;
        var configAsString = await retriever.GetDocumentAsync(configAddress, ct);

        var oidcConfig = JsonSerializer.Deserialize<OidcJwtSigningKeysConfig>(configAsString)!;
        if (!string.IsNullOrEmpty(oidcConfig.JwksUri))
        {
            var keys = await retriever.GetDocumentAsync(oidcConfig.JwksUri, ct).ConfigureAwait(false);

            var keySet = JsonSerializer.Deserialize<JwtSigningKeySet>(keys)!;

            foreach (var key in keySet.Keys)
            {
                if (key.Kid != kid) continue;

                var rsaParams = new RSAParameters
                {
                    Modulus = Base64UrlEncoder.DecodeBytes(key.N),
                    Exponent = Base64UrlEncoder.DecodeBytes(key.E)
                };
                _jwtSigningKeysCache.Add(kid, new RsaSecurityKey(rsaParams));
            }
        }
    }

    private static string Base64Decode(string token)
    {
        token = token.Replace('_', '/').Replace('-', '+');
        switch (token.Length % 4)
        {
            case 2: token += "=="; break;
            case 3: token += "="; break;
        }       
        var decoded = Convert.FromBase64String(token);
        return Encoding.Default.GetString(decoded);
    }
}