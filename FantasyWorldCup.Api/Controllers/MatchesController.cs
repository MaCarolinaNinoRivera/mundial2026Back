using Microsoft.AspNetCore.Mvc;
using FantasyWorldCup.Application.Matches.Queries.GetMatches;
using Microsoft.AspNetCore.Authorization;

namespace FantasyWorldCup.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly GetMatchResultsHandler _resultsHandler;

    public MatchesController(GetMatchResultsHandler resultsHandler)
    {
        _resultsHandler = resultsHandler;
    }

    /// <summary>
    /// Obtiene todos los resultados de los partidos con información de grupos y países.
    /// </summary>
    [HttpGet("results")]
    [AllowAnonymous] // Permitimos que usuarios no logueados vean los resultados
    public async Task<ActionResult<List<MatchResultDto>>> GetResults([FromQuery] DateTime? date)
    {
        var results = await _resultsHandler.Handle(date);

        if (results == null || !results.Any())
        {
            return Ok(new
            {
                message = $"No se encontraron partidos para la fecha {(date ?? DateTime.UtcNow):dd/MM/yyyy}.",
                data = new List<MatchResultDto>()
            });
        }

        return Ok(results);
    }
}
