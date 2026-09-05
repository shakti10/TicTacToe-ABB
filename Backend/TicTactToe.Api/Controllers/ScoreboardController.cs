
namespace TicTactToe.Api.Controllers;

/// <summary>
/// API endpoints for scoreboard management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ScoreboardController : ControllerBase
{
    private readonly GameStore gameStore;

    public ScoreboardController(GameStore gameStore)
    {
        this.gameStore = gameStore;
    }

    /// <summary>
    /// Get the current session scoreboard.
    /// </summary>
    /// <returns>The scoreboard with win/draw counts</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<ScoreboardDto> GetScoreboard()
    {
        var scoreboard = gameStore.GetScoreboard();
        return Ok(new ScoreboardDto
        {
            XWins = scoreboard.XWins,
            OWins = scoreboard.OWins,
            Draws = scoreboard.Draws
        });
    }

    /// <summary>
    /// Reset the session scoreboard to zero.
    /// </summary>
    /// <returns>The reset scoreboard</returns>
    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<ScoreboardDto> ResetScoreboard()
    {
        gameStore.ResetScoreboard();
        var scoreboard = gameStore.GetScoreboard();
        return Ok(new ScoreboardDto
        {
            XWins = scoreboard.XWins,
            OWins = scoreboard.OWins,
            Draws = scoreboard.Draws
        });
    }
}
