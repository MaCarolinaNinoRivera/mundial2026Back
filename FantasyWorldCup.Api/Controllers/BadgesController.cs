using FantasyWorldCup.Application.Badges.Queries.GetBadges;
using Microsoft.AspNetCore.Mvc;

namespace FantasyWorldCup.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BadgesController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BadgeDto>>> GetAll([FromServices] GetBadgesHandler handler)
    {
        var badges = await handler.Handle();
        return Ok(badges);
    }
}
