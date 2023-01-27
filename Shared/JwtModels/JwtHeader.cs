using System.Text.Json.Serialization;

namespace Shared.JwtModels;

public class JwtHeader
{
    [JsonPropertyName("kid")] public string Kid { get; set; } = string.Empty;
}