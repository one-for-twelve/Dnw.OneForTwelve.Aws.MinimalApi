using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Dnw.OneForTwelve.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using Shared;
using Shared.Clients;

namespace Api;

internal static class Handlers
{
    internal static ILogger? Logger;
    internal static IGameClient? GameClient;
    private static readonly ConcurrentDictionary<string, SecurityKey> SecurityKeysByKid = new();

    public static async Task Default(HttpContext context)
    {
        var userName = context.User.Identity?.Name ?? "anonymous";
        var architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture;
        var dotnetVersion = Environment.Version.ToString();
        var body = $"Username: {userName}, Env: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}, Architecture: {architecture}, .NET Version: {dotnetVersion}";
        
        await context.WriteResponse(HttpStatusCode.OK, body);
    }
    
    public static async Task StartGame(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.ToString();
        Logger?.LogInformation("{authHeader}", authHeader);

        const string authority = "https://securetoken.google.com/one-for-twelve-32778";
        
        // var validIssuers = new List<string> { authority };
        //
        // var configManager = new ConfigurationManager<OpenIdConnectConfiguration>($"{validIssuers.Last()}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
        //
        // var oidcConfig = configManager.GetConfigurationAsync().Result;
        
        var token = authHeader.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
        
        var headerBase64 = token.Split('.')[0];
        var headerJson = Base64Decode(headerBase64);
        var header = JsonSerializer.Deserialize<JsonWebHeader>(headerJson);
        var kid = header!.Kid;

        if (!SecurityKeysByKid.ContainsKey(kid))
        {
            const string configAddress = $"{authority}/.well-known/openid-configuration";
            var retriever = new HttpDocumentRetriever();
            var ct = CancellationToken.None;
            var configAsString = await retriever.GetDocumentAsync(configAddress, ct);
        
            var oidcConfig = JsonSerializer.Deserialize<OidcConfigCustom>(configAsString)!;
            if (!string.IsNullOrEmpty(oidcConfig.JwksUri))
            {
                var keys = await retriever.GetDocumentAsync(oidcConfig.JwksUri, ct).ConfigureAwait(false);
            
                var keySet = JsonSerializer.Deserialize<JsonWebKeySetCustom>(keys)!;

                foreach (var key in keySet.Keys)
                {
                    if (key.Kid != kid) continue;
                    
                    var rsaParams = new RSAParameters
                    {
                        Modulus = Base64UrlEncoder.DecodeBytes(key.N),
                        Exponent = Base64UrlEncoder.DecodeBytes(key.E)
                    };
                    SecurityKeysByKid.TryAdd(kid, new RsaSecurityKey(rsaParams));
                }
            }
        }
        
        // try
        // {
        //     var tokenValidationParams = new TokenValidationParameters
        //     {
        //         ValidateIssuer = true,
        //         ValidIssuer = authority,
        //         
        //         ValidateAudience = true,
        //         ValidAudience = "one-for-twelve-32778",
        //         
        //         ValidateIssuerSigningKey = true,
        //         IssuerSigningKeys = new [] { SecurityKeysByKid[kid] },
        //         
        //         ValidateLifetime = true
        //     };
        //     var jsonWebTokenHandler = new JsonWebTokenHandler();
        //     var tokenValidationResult = jsonWebTokenHandler.ValidateToken(token, tokenValidationParams);
        //     if (!tokenValidationResult.IsValid)
        //     {
        //         // Handle each exception which tokenValidationResult can contain as appropriate for your service
        //         // Your service might need to respond with a http response instead of an exception.
        //         if (tokenValidationResult.Exception != null)
        //             throw tokenValidationResult.Exception;
        //
        //         throw new ApplicationException("Token Validation Failed");
        //     }
        //     
        //     var uid = tokenValidationResult.ClaimsIdentity.Name;
        //     Logger?.LogInformation("{uid}", uid);
        // }
        // catch (Exception ex)
        // {
        //     Logger?.LogError(ex, "{message}", ex.Message);
        //     throw;
        // }

        var languageAsString = context.Request.RouteValues["language"]?.ToString()!;
        var strategyAsString = context.Request.RouteValues["questionSelectionStrategy"]?.ToString()!;

        var language = Enum.Parse<Languages>(languageAsString);
        var strategy = Enum.Parse<QuestionSelectionStrategies>(strategyAsString);
        
        var game = GameClient!.Start(language, strategy)!;
        
        Logger?.LogInformation($"Game started: {game.Word}");
        
        await context.WriteResponse(HttpStatusCode.OK, game);
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