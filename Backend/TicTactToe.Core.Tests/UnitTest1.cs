using TicTactToe.Core;

namespace TicTactToe.Core.Tests;

public class TicTacToeGameTests
{
    [Fact]
    public void ValidMoveAddsHistoryAndSwitchesTurn()
    {
        var game = new TicTacToeGame(GameMode.TwoPlayer);

        Assert.True(game.TryMove(Player.X, 1, 1, out _));
        Assert.Equal(Player.X, game.GetCell(1, 1));
        Assert.Equal(Player.O, game.CurrentPlayer);
        Assert.Single(game.Moves);
    }

    [Fact]
    public void InvalidMoveDoesNotChangeState()
    {
        var game = new TicTacToeGame(GameMode.TwoPlayer);
        game.TryMove(Player.X, 1, 1, out _);

        Assert.False(game.TryMove(Player.X, 1, 1, out _));
        Assert.Single(game.Moves);
        Assert.Equal(Player.O, game.CurrentPlayer);
        Assert.False(game.TryMove(Player.O, 4, 1, out _));
        Assert.Single(game.Moves);
    }

    [Fact]
    public void DetectsRowWinAndUpdatesScoreboard()
    {
        var scoreboard = new Scoreboard();
        var game = new TicTacToeGame(GameMode.TwoPlayer, scoreboard);
        Play(game, Player.X, 1, 1);
        Play(game, Player.O, 2, 1);
        Play(game, Player.X, 1, 2);
        Play(game, Player.O, 2, 2);

        Play(game, Player.X, 1, 3);

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Player.X, game.Winner);
        Assert.Equal(3, game.WinningCells.Count);
        Assert.Equal(1, scoreboard.XWins);
    }

    [Fact]
    public void DetectsColumnWin()
    {
        var game = new TicTacToeGame(GameMode.TwoPlayer);
        Play(game, Player.X, 1, 1);
        Play(game, Player.O, 1, 2);
        Play(game, Player.X, 2, 1);
        Play(game, Player.O, 2, 2);
        Play(game, Player.X, 3, 1);

        Assert.Equal(Player.X, game.Winner);
        Assert.Equal(GameStatus.Won, game.Status);
    }

    [Fact]
    public void DetectsDiagonalWin()
    {
        var game = new TicTacToeGame(GameMode.TwoPlayer);
        Play(game, Player.X, 1, 1);
        Play(game, Player.O, 1, 2);
        Play(game, Player.X, 2, 2);
        Play(game, Player.O, 1, 3);
        Play(game, Player.X, 3, 3);

        Assert.Equal(Player.X, game.Winner);
        Assert.Contains(new Cell(2, 2), game.WinningCells);
    }

    [Fact]
    public void DetectsDrawAndUpdatesScoreboardOnce()
    {
        var scoreboard = new Scoreboard();
        var game = new TicTacToeGame(GameMode.TwoPlayer, scoreboard);
        Play(game, Player.X, 1, 1);
        Play(game, Player.O, 1, 3);
        Play(game, Player.X, 1, 2);
        Play(game, Player.O, 2, 1);
        Play(game, Player.X, 2, 3);
        Play(game, Player.O, 3, 2);
        Play(game, Player.X, 3, 1);
        Play(game, Player.O, 2, 2);
        Play(game, Player.X, 3, 3);

        Assert.Equal(GameStatus.Draw, game.Status);
        Assert.Equal(1, scoreboard.Draws);
        Assert.False(game.TryMove(Player.O, 1, 1, out _));
        Assert.Equal(1, scoreboard.Draws);
    }

    [Fact]
    public void UndoRemovesOneMoveInTwoPlayerMode()
    {
        var game = new TicTacToeGame(GameMode.TwoPlayer);
        Play(game, Player.X, 1, 1);
        Play(game, Player.O, 2, 2);

        Assert.True(game.Undo(out _));
        Assert.Null(game.GetCell(2, 2));
        Assert.Equal(Player.O, game.CurrentPlayer);
        Assert.Single(game.Moves);
    }

    [Fact]
    public void UndoRemovesHumanAndComputerPair()
    {
        var game = new TicTacToeGame(GameMode.Computer);
        Assert.True(game.TryMove(Player.X, 1, 1, out _));
        Assert.Equal(2, game.Moves.Count);

        Assert.True(game.Undo(out _));
        Assert.Empty(game.Moves);
        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.Null(game.GetCell(1, 1));
    }

    [Fact]
    public void ComputerTakesCenterWhenAvailable()
    {
        var game = new TicTacToeGame(GameMode.Computer);

        Assert.True(game.TryMove(Player.X, 1, 1, out _));

        Assert.Equal(Player.O, game.GetCell(2, 2));
        Assert.Equal(Player.X, game.CurrentPlayer);
    }

    [Fact]
    public void ResetClearsGameButPreservesScoreboard()
    {
        var scoreboard = new Scoreboard();
        var game = new TicTacToeGame(GameMode.TwoPlayer, scoreboard);
        Play(game, Player.X, 1, 1);
        Play(game, Player.O, 2, 1);
        Play(game, Player.X, 1, 2);
        Play(game, Player.O, 2, 2);
        Play(game, Player.X, 1, 3);

        game.Reset();

        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.Empty(game.Moves);
        Assert.Equal(1, scoreboard.XWins);
    }

    private static void Play(TicTacToeGame game, Player player, int row, int column)
    {
        Assert.True(game.TryMove(player, row, column, out var error), error);
    }
}