using Microsoft.AspNetCore.Mvc;
using FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard;

namespace FantasyWorldCup.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly GetLeaderboardHandler _handler;

    public LeaderboardController(GetLeaderboardHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<ActionResult<List<LeaderboardDto>>> Get()
    {
        return Ok(await _handler.Handle());
    }

    [HttpGet("badges")]
    public async Task<IActionResult> GetBadgesLeaderboard([FromServices] GetBadgesLeaderboardHandler handler)
    {
        var result = await handler.Handle();
        return Ok(result);
    }

    [HttpGet("trends")]
    public async Task<ActionResult<List<RankMovementDto>>> GetTrends()
    {
        var trends = await _handler.Handle();
        return Ok(trends);
    }
}
