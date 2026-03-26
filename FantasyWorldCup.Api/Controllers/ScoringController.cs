using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using FantasyWorldCup.Application.Scoring.Queries.GetScoringRules; // <--- Aquí vive el Handler y el DTO

namespace FantasyWorldCup.Api.Controllers;

[ApiController]
[Route("api/scoring-rules")]
public class ScoringController : ControllerBase
{
    private readonly GetScoringRulesHandler _handler;

    public ScoringController(GetScoringRulesHandler handler) => _handler = handler;

    [HttpGet]
    public async Task<IActionResult> GetRules()
    {
        var rules = await _handler.Handle();
        return Ok(rules);
    }
}