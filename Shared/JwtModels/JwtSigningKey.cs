using System.Text.Json.Serialization;

namespace Shared.JwtModels;

public class JwtSigningKey
{
    [JsonPropertyName("n")] public string N { get; set; } = string.Empty;
    [JsonPropertyName("e")] public string E { get; set; } = string.Empty;
    [JsonPropertyName("kid")]  public string Kid { get; set; } = string.Empty;
}