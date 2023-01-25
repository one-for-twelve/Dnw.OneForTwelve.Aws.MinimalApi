using System.Net;
using System.Text.Json;
using Dnw.OneForTwelve.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Clients;

var app = Startup.Build(args);

Handlers.Logger = app.Logger;
Handlers.GameClient = app.Services.GetRequiredService<IGameClient>();

app.MapGet("/", Handlers.Default);
var startGameEndpoint = app.MapGet("/games/{language}/{questionSelectionStrategy}", Handlers.StartGame);

Startup.RequireAuthorization(new[] { startGameEndpoint });

app.Run();

static class Handlers
{
    internal static ILogger? Logger;
    internal static IGameClient? GameClient;

    public static async Task Default(HttpContext context)
    {
        var userName = context.User.Identity?.Name ?? "anonymous";
        var architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture;
        var dotnetVersion = Environment.Version.ToString();
        var body = $"Username: {userName}, Env: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}, Architecture: {architecture}, .NET Version: {dotnetVersion}";
        
        await context.WriteResponse(HttpStatusCode.OK, body);
    }
    
    public static async Task StartGame(HttpContext context)
    {
        var languageAsString = context.Request.RouteValues["language"]?.ToString()!;
        var strategyAsString = context.Request.RouteValues["questionSelectionStrategy"]?.ToString()!;

        var language = Enum.Parse<Languages>(languageAsString);
        var strategy = Enum.Parse<QuestionSelectionStrategies>(strategyAsString);
        
        var game = GameClient!.Start(language, strategy)!;
        
        Logger?.LogInformation($"Game started: {game.Word}");
        
        await context.WriteResponse(HttpStatusCode.OK, game);
    }
}

internal static class ResponseWriter
{
    public static async Task WriteResponse<TResponseType>(this HttpContext context, HttpStatusCode statusCode, TResponseType body) where TResponseType : class
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, typeof(TResponseType), ApiSerializerContext.Default));
    }
}