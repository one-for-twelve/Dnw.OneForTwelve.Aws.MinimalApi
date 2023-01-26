using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;
using Dnw.OneForTwelve.Core.Models;

namespace Shared;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
[JsonSerializable(typeof(Game))]
[JsonSerializable(typeof(OidcConfigCustom))]
[JsonSerializable(typeof(JsonWebKeySetCustom))]
[JsonSerializable(typeof(JsonWebKeyCustom))]
[JsonSerializable(typeof(JsonWebHeader))]
public partial class ApiSerializerContext : JsonSerializerContext
{
}