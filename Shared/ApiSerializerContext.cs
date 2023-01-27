using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;
using Dnw.OneForTwelve.Core.Models;
using Shared.JwtModels;

namespace Shared;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
[JsonSerializable(typeof(Game))]
[JsonSerializable(typeof(OidcJwtSigningKeysConfig))]
[JsonSerializable(typeof(JwtSigningKeySet))]
[JsonSerializable(typeof(JwtSigningKey))]
[JsonSerializable(typeof(JwtHeader))]
public partial class ApiSerializerContext : JsonSerializerContext
{
}