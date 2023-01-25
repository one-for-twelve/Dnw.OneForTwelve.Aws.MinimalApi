using Amazon.Lambda.Serialization.SystemTextJson;
using Dnw.OneForTwelve.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Clients;

namespace Shared
{
    public static class Startup
    {
        public static WebApplication Build(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddFirebaseAuth();
                        
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.AddContext<ApiSerializerContext>();
            });
            
            builder.Services.AddGameServices();
            builder.Services.AddSingleton<IGameClient, GameClient>();
            
            builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi, options =>
            {
                options.Serializer = new SourceGeneratorLambdaJsonSerializer<ApiSerializerContext>();
            });
            
            builder.Logging.ClearProviders();
            builder.Logging.AddJsonConsole(options =>
            {
                options.IncludeScopes = true;
                options.UseUtcTimestamp = true;
                options.TimestampFormat = "hh:mm:ss ";
            });

            var app = builder.Build();
            
            // Add generic app configuration here.

            return app;
        }
        
        public static void RequireAuthorization(IEnumerable<IEndpointConventionBuilder> endpointBuilders)
        {
            foreach (var endpointBuilder in endpointBuilders)
            {
                endpointBuilder.RequireAuthorization();
            }
        }
    }
}