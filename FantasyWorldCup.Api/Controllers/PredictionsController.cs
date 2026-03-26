using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FantasyWorldCup.Application.Predictions.UseCases.UpsertPrediction;
using FantasyWorldCup.Application.Predictions.UseCases.GetMyPredictions;
using FantasyWorldCup.Application.Predictions.UseCases.CalculatePoints;
using FantasyWorldCup.Application.Matches.Queries.GetTodayMatches;
using FantasyWorldCup.Application.Matches.Queries.GetMatches;

namespace FantasyWorldCup.Api.Controllers;

[ApiController]
[Route("api/predictions")]
[Authorize]
public class PredictionsController : ControllerBase
{
    private readonly UpsertPredictionHandler _handler;
    private readonly GetMyPredictionsHandler _getHandler;
    private readonly CalculatePointsHandler _pointsHandler;
    private readonly GetTodayMatchesHandler _todayMatchesHandler;

    public PredictionsController(UpsertPredictionHandler handler,
        GetMyPredictionsHandler getHandler, CalculatePointsHandler pointsHandler,
        GetTodayMatchesHandler todayMatchesHandler)
    {
        _handler = handler;
        _getHandler = getHandler;
        _pointsHandler = pointsHandler;
        _todayMatchesHandler = todayMatchesHandler;
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetTodayMatches()
    {
        var matches = await _todayMatchesHandler.Handle();

        if (matches == null || !matches.Any())
        {
            // Devolvemos un 200 con mensaje amigable o un 404/204 según prefieras
            return Ok(new { message = "Hoy no hay partidos. ¡Regresa mañana!", data = new List<MatchDto>() });
        }

        return Ok(matches);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(UpsertPredictionCommand command)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _handler.Handle(userId, command);

        return Ok(response);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyPredictions()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _getHandler.Handle(userId);
        return Ok(result);
    }

    [HttpPost("{id}/calculate-points")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CalculatePoints(Guid id)
    {
        await _pointsHandler.Handle(id);

        return Ok(new { message = "Puntos procesados exitosamente para este partido." });
    }
}
