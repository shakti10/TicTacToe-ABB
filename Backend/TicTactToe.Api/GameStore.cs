using TicTactToe.Core;

namespace TicTactToe.Api;

public class GameStore
{
    private readonly Dictionary<Guid, TicTacToeGame> games = new();
    private readonly Scoreboard scoreboard = new();

    public TicTacToeGame CreateGame(GameMode mode)
    {
        var game = new TicTacToeGame(mode, scoreboard);
        games[game.GameId] = game;
        return game;
    }

    public TicTacToeGame? GetGame(Guid gameId)
    {
        games.TryGetValue(gameId, out var game);
        return game;
    }

    public Scoreboard GetScoreboard() => scoreboard;

    public void ResetScoreboard() => scoreboard.Reset();
}
