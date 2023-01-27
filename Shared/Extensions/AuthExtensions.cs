using Microsoft.AspNetCore.Http;
using Shared.Services;

namespace Shared.Extensions;

public static class AuthExtensions
{
    public static async Task EnsureAuthenticated(this HttpContext httpContext, string authority)
    {
        await EnsureAuthenticated(new JwtValidator(httpContext, authority, new JwtSigningKeysCache()));
    }

    internal static async Task EnsureAuthenticated(IJwtValidator jwtValidator)
    {
        await jwtValidator.Validate();
    }
}