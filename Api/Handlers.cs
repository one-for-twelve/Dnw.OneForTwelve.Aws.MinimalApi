using System.Net;
using Dnw.OneForTwelve.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Clients;
using Shared.Extensions;

namespace Api;

internal static class Handlers
{
    private const string Authority = "https://securetoken.google.com/one-for-twelve-32778";
    
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
        await context.EnsureAuthenticated(Authority);

        var languageAsString = context.Request.RouteValues["language"]?.ToString()!;
        var strategyAsString = context.Request.RouteValues["questionSelectionStrategy"]?.ToString()!;

        var language = Enum.Parse<Languages>(languageAsString);
        var strategy = Enum.Parse<QuestionSelectionStrategies>(strategyAsString);
        
        var game = GameClient!.Start(language, strategy)!;
        
        Logger?.LogInformation("Game started for {userName}: {word}", context.User.Identity?.Name, game.Word);
        
        await context.WriteResponse(HttpStatusCode.OK, game);
    }
}