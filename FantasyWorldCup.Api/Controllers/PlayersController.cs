using FantasyWorldCup.Application.Countries.Interfaces;
using FantasyWorldCup.Application.Countries.Queries.GetFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FantasyWorldCup.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly ICountryRepository _repository;

    public PlayersController(ICountryRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Lista los futbolistas disponibles con filtros opcionales por país, grupo o posición.
    /// </summary>
    /// <param name="countryId">ID opcional del país</param>
    /// <param name="groupId">ID opcional del grupo</param>
    /// <param name="position">Posición (Forward, Midfielder, Defender, Goalkeeper)</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<PlayerListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlayers(
        [FromQuery] Guid? countryId,
        [FromQuery] Guid? groupId,
        [FromQuery] string? position)
    {
        var players = await _repository.GetFilteredPlayersAsync(countryId, groupId, position);
        return Ok(players);
    }
}
