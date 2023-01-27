using System.Text.Json.Serialization;

namespace Shared.JwtModels;

public class JwtSigningKeySet
{
    [JsonPropertyName("keys")] public IList<JwtSigningKey> Keys { get; set; } = new List<JwtSigningKey>();
}