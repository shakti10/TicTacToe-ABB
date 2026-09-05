using TicTactToe.Core;

namespace TicTactToe.Api.Models;

/// <summary>
/// Request to create a new game.
/// </summary>
public class CreateGameRequest
{
    /// <summary>
    /// Game mode: "TwoPlayer" or "Computer"
    /// </summary>
    public string Mode { get; set; } = "TwoPlayer";
}

/// <summary>
/// Request to submit a move.
/// </summary>
public class MoveRequest
{
    /// <summary>
    /// Player making the move: "X" or "O"
    /// </summary>
    public string Player { get; set; } = "X";

    /// <summary>
    /// Row number (1-3)
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// Column number (1-3)
    /// </summary>
    public int Column { get; set; }
}

/// <summary>
/// Represents a single move in the game history.
/// </summary>
public class MoveDto
{
    public int MoveNumber { get; set; }
    public string Player { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Column { get; set; }
}

/// <summary>
/// Represents a cell coordinate.
/// </summary>
public class CellDto
{
    public int Row { get; set; }
    public int Column { get; set; }
}

/// <summary>
/// Represents the game board state.
/// </summary>
public class BoardDto
{
    /// <summary>
    /// 3x3 grid of cells. Each cell is "X", "O", or null.
    /// </summary>
    public string?[][] Grid { get; set; } = new string?[3][];
}

/// <summary>
/// Represents the game scoreboard.
/// </summary>
public class ScoreboardDto
{
    public int XWins { get; set; }
    public int OWins { get; set; }
    public int Draws { get; set; }
}

/// <summary>
/// Represents the complete game state.
/// </summary>
public class GameStateDto
{
    public Guid GameId { get; set; }
    public string Mode { get; set; } = string.Empty;
    public string CurrentPlayer { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Winner { get; set; }
    public List<CellDto> WinningCells { get; set; } = new();
    public List<MoveDto> MoveHistory { get; set; } = new();
    public BoardDto Board { get; set; } = new();
    public ScoreboardDto Scoreboard { get; set; } = new();
}

/// <summary>
/// Error response.
/// </summary>
public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
}
