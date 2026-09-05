
namespace TicTactToe.Api.Controllers;

/// <summary>
/// API endpoints for Tic Tac Toe game management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly GameStore gameStore;

    public GamesController(GameStore gameStore)
    {
        this.gameStore = gameStore;
    }

    /// <summary>
    /// Create a new game session.
    /// </summary>
    /// <param name="request">Request with game mode (TwoPlayer or Computer)</param>
    /// <returns>The created game state</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<GameStateDto> CreateGame([FromBody] CreateGameRequest request)
    {
        if (!Enum.TryParse<GameMode>(request.Mode, out var mode))
        {
            return BadRequest(new ErrorResponse { Message = "Invalid game mode. Use 'TwoPlayer' or 'Computer'." });
        }

        var game = gameStore.CreateGame(mode);
        return CreatedAtAction(nameof(GetGame), new { gameId = game.GameId }, GameMapper.ToDto(game));
    }

    /// <summary>
    /// Get the current state of a game.
    /// </summary>
    /// <param name="gameId">The game ID</param>
    /// <returns>The game state</returns>
    [HttpGet("{gameId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<GameStateDto> GetGame(Guid gameId)
    {
        var game = gameStore.GetGame(gameId);
        if (game == null)
        {
            return NotFound(new ErrorResponse { Message = "Game not found." });
        }

        return Ok(GameMapper.ToDto(game));
    }

    /// <summary>
    /// Submit a move in a game.
    /// </summary>
    /// <param name="gameId">The game ID</param>
    /// <param name="request">The move details (player, row, column)</param>
    /// <returns>The updated game state</returns>
    [HttpPost("{gameId}/moves")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<GameStateDto> SubmitMove(Guid gameId, [FromBody] MoveRequest request)
    {
        var game = gameStore.GetGame(gameId);
        if (game == null)
        {
            return NotFound(new ErrorResponse { Message = "Game not found." });
        }

        if (!Enum.TryParse<Player>(request.Player, out var player))
        {
            return BadRequest(new ErrorResponse { Message = "Invalid player. Use 'X' or 'O'." });
        }

        if (!game.TryMove(player, request.Row, request.Column, out var error))
        {
            return BadRequest(new ErrorResponse { Message = error });
        }

        return Ok(GameMapper.ToDto(game));
    }

    /// <summary>
    /// Undo the last move(s) in a game.
    /// </summary>
    /// <param name="gameId">The game ID</param>
    /// <returns>The updated game state</returns>
    [HttpPost("{gameId}/undo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<GameStateDto> Undo(Guid gameId)
    {
        var game = gameStore.GetGame(gameId);
        if (game == null)
        {
            return NotFound(new ErrorResponse { Message = "Game not found." });
        }

        if (!game.Undo(out var error))
        {
            return BadRequest(new ErrorResponse { Message = error });
        }

        return Ok(GameMapper.ToDto(game));
    }

    /// <summary>
    /// Reset a game to initial state while preserving scoreboard.
    /// </summary>
    /// <param name="gameId">The game ID</param>
    /// <returns>The reset game state</returns>
    [HttpPost("{gameId}/reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<GameStateDto> ResetGame(Guid gameId)
    {
        var game = gameStore.GetGame(gameId);
        if (game == null)
        {
            return NotFound(new ErrorResponse { Message = "Game not found." });
        }

        game.Reset();
        return Ok(GameMapper.ToDto(game));
    }
}
