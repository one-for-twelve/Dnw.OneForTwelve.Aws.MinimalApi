using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;
using Dnw.OneForTwelve.Core.Models;

namespace Shared;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
[JsonSerializable(typeof(Game))]
public partial class ApiSerializerContext : JsonSerializerContext
{
}