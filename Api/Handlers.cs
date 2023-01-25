using System.Net;
using Dnw.OneForTwelve.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Clients;

namespace Api;

internal static class Handlers
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