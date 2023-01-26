using System.Text.Json.Serialization;

namespace Shared;

public class JsonWebKeySetCustom
{
    [JsonPropertyName("keys")] public IList<JsonWebKeyCustom> Keys { get; set; } = new List<JsonWebKeyCustom>();
}

public class JsonWebKeyCustom
{
    [JsonPropertyName("n")] public string N { get; set; } = string.Empty;
    [JsonPropertyName("e")] public string E { get; set; } = string.Empty;
    [JsonPropertyName("kid")]  public string Kid { get; set; } = string.Empty;
}

public class JsonWebHeader
{
    [JsonPropertyName("kid")] public string Kid { get; set; } = string.Empty;
}

public class OidcConfigCustom
{
    [JsonPropertyName("jwks_uri")] public string JwksUri { get; set; } = string.Empty;
}