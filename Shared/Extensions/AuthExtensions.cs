using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Extensions;

public static class AuthExtensions
{
    public static void DoAddFirebaseAuth(this IServiceCollection services)
    {
        services.AddAuthentication("Bearer").AddJwtBearer(options =>
        {
            options.Authority = "https://securetoken.google.com/one-for-twelve-32778";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "https://securetoken.google.com/one-for-twelve-32778",
                ValidateAudience = true,
                ValidAudience = "one-for-twelve-32778",
                ValidateLifetime = true
            };
        });
        services.AddAuthorization(o => o.AddPolicy("NotAnonymous", b => b.RequireAuthenticatedUser()));

    }
}