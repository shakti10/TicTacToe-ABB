using TicTactToe.Core;

namespace TicTactToe.Api;

public static class GameMapper
{
    public static GameStateDto ToDto(TicTacToeGame game)
    {
        return new GameStateDto
        {
            GameId = game.GameId,
            Mode = game.Mode.ToString(),
            CurrentPlayer = game.CurrentPlayer.ToString(),
            Status = game.Status.ToString(),
            Winner = game.Winner?.ToString(),
            WinningCells = game.WinningCells
                .Select(c => new Models.CellDto { Row = c.Row, Column = c.Column })
                .ToList(),
            MoveHistory = game.Moves
                .Select(m => new Models.MoveDto
                {
                    MoveNumber = m.MoveNumber,
                    Player = m.Player.ToString(),
                    Row = m.Row,
                    Column = m.Column
                })
                .ToList(),
            Board = GetBoardDto(game),
            Scoreboard = new Models.ScoreboardDto
            {
                XWins = game.Scoreboard.XWins,
                OWins = game.Scoreboard.OWins,
                Draws = game.Scoreboard.Draws
            }
        };
    }

    private static Models.BoardDto GetBoardDto(TicTacToeGame game)
    {
        var grid = new string?[3][];
        for (int row = 1; row <= 3; row++)
        {
            grid[row - 1] = new string?[3];
            for (int col = 1; col <= 3; col++)
            {
                var cell = game.GetCell(row, col);
                grid[row - 1][col - 1] = cell?.ToString();
            }
        }
        return new Models.BoardDto { Grid = grid };
    }
}
