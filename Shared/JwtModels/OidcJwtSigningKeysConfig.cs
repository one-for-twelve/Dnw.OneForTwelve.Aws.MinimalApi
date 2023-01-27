using System.Text.Json.Serialization;

namespace Shared.JwtModels;

public class OidcJwtSigningKeysConfig
{
    [JsonPropertyName("jwks_uri")] public string JwksUri { get; set; } = string.Empty;
}