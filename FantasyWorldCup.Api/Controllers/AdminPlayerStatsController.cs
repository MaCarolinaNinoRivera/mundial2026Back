using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FantasyWorldCup.Application.Matches.Validators;
using FantasyWorldCup.Application.Matches.UseCases.DistributePoints;
using FantasyWorldCup.Application.Matches.UseCases.RecordMatchStats; // <--- Asegúrate de que esté este using
using FantasyWorldCup.Application.Matches.UseCases.RecordMatchEvent;

namespace FantasyWorldCup.Api.Controllers;

// ELIMINA la clase PlayerStatEntry de aquí (la que tenías arriba del controlador)
// Porque ahora usaremos la que está dentro de FantasyWorldCup.Application.Matches.UseCases.RecordMatchStats

[ApiController]
[Route("api/admin/matches")]
[Authorize(Roles = "Admin")]
public class AdminPlayerStatsController : ControllerBase
{
    private readonly RecordMatchStatsHandler _recordStatsHandler; // <--- El nombre debe coincidir con el error
    private readonly DistributePointsHandler _distributeHandler;

    public AdminPlayerStatsController(
        RecordMatchStatsHandler recordStatsHandler,
        DistributePointsHandler distributeHandler)
    {
        _recordStatsHandler = recordStatsHandler; // <--- ASIGNACIÓN CORRECTA
        _distributeHandler = distributeHandler;
    }

    [HttpPost("{matchId}/stats")]
    public async Task<IActionResult> RecordStats(Guid matchId, [FromBody] List<PlayerStatEntry> stats)
    {
        // Ahora PlayerStatEntry será el de la capa Application automáticamente por el using
        var command = new RecordMatchStatsCommand(matchId, stats);
        await _recordStatsHandler.Handle(command);

        return Ok(new { message = "Estadísticas registradas correctamente." });
    }

    [HttpPost("{matchId}/distribute-points")]
    public async Task<IActionResult> DistributePoints(Guid matchId)
    {
        await _distributeHandler.Handle(matchId);
        return Ok(new { message = "Puntos distribuidos exitosamente." });
    }

    [HttpPost("matches/{matchId}/events")]
    public async Task<IActionResult> RecordEvent(
    Guid matchId,
    [FromBody] RecordEventRequest request,
    [FromServices] RecordMatchEventHandler handler)
    {
        await handler.Handle(new RecordMatchEventCommand(
            matchId,
            request.PlayerId,
            request.EventType,
            request.Minute,
            request.RelatedPlayerId
        ));

        return Ok(new { message = "Evento registrado correctamente." });
    }

    // DTO simple para el Body
    public record RecordEventRequest(Guid PlayerId, string EventType, int Minute, Guid? RelatedPlayerId = null);
}
