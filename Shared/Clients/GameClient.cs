using Dnw.OneForTwelve.Core.Models;
using Dnw.OneForTwelve.Core.Services;

namespace Shared.Clients;

public interface IGameClient
{
    Game? Start(Languages language, QuestionSelectionStrategies strategy);
}

public class GameClient : IGameClient
{
    private readonly IGameService _gameService;

    public GameClient(IGameService gameService)
    {
        _gameService = gameService;
    }
    
    public Game? Start(Languages language, QuestionSelectionStrategies strategy)
    {
        return _gameService.Start(language, strategy);
    }
}