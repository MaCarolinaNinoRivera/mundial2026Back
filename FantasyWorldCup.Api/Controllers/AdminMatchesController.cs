using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FantasyWorldCup.Application.Matches.UseCases.CreateMatch;
using FantasyWorldCup.Application.Matches.UseCases.UpdateMatch;
using FantasyWorldCup.Application.Teams.UseCases.LockAllTeams;
using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Application.Scoring.UseCases.RecordRankSnapshot; // <-- ESTE ES EL QUE FALTA
using FantasyWorldCup.Application.Matches.Queries.GetMatchPlayers;

namespace FantasyWorldCup.Api.Controllers;

[ApiController]
[Route("api/matches")]
[Authorize(Roles = "Admin")]
public class AdminMatchesController : ControllerBase
{
    private readonly CreateMatchHandler _createHandler;
    private readonly IMatchRepository _repository;
    private readonly UpdateMatchHandler _handler;
    private readonly LockAllTeamsHandler _lockHandler;

    public AdminMatchesController(
        CreateMatchHandler createHandler,
        IMatchRepository repository,
        UpdateMatchHandler updateHandler,
        LockAllTeamsHandler lockHandler)
    {
        _createHandler = createHandler;
        _repository = repository;
        _handler = updateHandler;
        _lockHandler = lockHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMatchCommand command)
    {
        var id = await _createHandler.Handle(command);
        return Ok(id);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var matches = await _repository.GetAllWithNamesAsync();
        return Ok(matches);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var matches = await _repository.GetByIdWithNamesAsync(id);
        return Ok(matches);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var match = await _repository.GetByIdAsync(id);
        if (match is null)
            return NotFound();

        _repository.Remove(match);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMatchCommand command)
    {
        await _handler.Handle(id, command);
        return NoContent();
    }

    [HttpPost("lock-all-teams")] // Cambié la ruta para que sea relativa al controller
    public async Task<IActionResult> LockTeams()
    {
        await _lockHandler.Handle();
        return Ok(new { message = "Mercado cerrado. Todos los equipos han sido bloqueados." });
    }

    [HttpPost("force-rank-snapshot")]
    public async Task<IActionResult> ForceSnapshot([FromServices] RecordRankSnapshotHandler handler)
    {
        await handler.ExecuteAsync();
        return Ok(new { message = "Snapshot ejecutado manualmente con éxito." });
    }

    [HttpGet("{matchId}/players")]
    public async Task<ActionResult<List<MatchPlayersDto>>> GetMatchPlayers(
    Guid matchId,
    [FromServices] GetMatchPlayersHandler handler)
    {
        return await handler.Handle(matchId);
    }
}
