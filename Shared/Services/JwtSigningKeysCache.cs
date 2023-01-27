using System.Collections.Concurrent;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Services;

public interface IJwtSigningKeysCache
{
    bool ContainsKey(string kid);
    void Add(string kid, SecurityKey key);
    SecurityKey GetKeyByKid(string kid);
}

public class JwtSigningKeysCache : IJwtSigningKeysCache
{
    private static readonly ConcurrentDictionary<string, SecurityKey> SecurityKeysByKid = new();

    public bool ContainsKey(string kid)
    {
        return SecurityKeysByKid.ContainsKey(kid);
    }

    public void Add(string kid, SecurityKey key)
    {
        SecurityKeysByKid.TryAdd(kid, key);
    }

    public SecurityKey GetKeyByKid(string kid)
    {
        return SecurityKeysByKid[kid];
    }
}