using Dnw.OneForTwelve.Core.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Extensions;

public static class AuthExtensions
{
    public static void AddFirebaseAuth(this IServiceCollection services)
    {
        services.AddAuthentication("Bearer").AddJwtBearer(options =>
        {
            options.Authority = JwtBearerConfig.Authority;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = JwtBearerConfig.Authority,
                ValidateAudience = true,
                ValidAudience = JwtBearerConfig.Audience,
                ValidateLifetime = true
            };
        });
        services.AddAuthorization(o => o.AddPolicy("NotAnonymous", b => b.RequireAuthenticatedUser()));

    }
}