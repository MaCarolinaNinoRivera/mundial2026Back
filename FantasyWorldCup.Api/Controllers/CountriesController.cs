using FantasyWorldCup.Application.Countries.Interfaces;
using FantasyWorldCup.Application.Countries.Queries.GetCountriesAndGroups;
using FantasyWorldCup.Application.Countries.Queries.GetFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FantasyWorldCup.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly ICountryRepository _repository;
    private readonly GetCountriesAndGroupsHandler _combinedHandler;

    public CountriesController(ICountryRepository repository, GetCountriesAndGroupsHandler combinedHandler)
    {
        _repository = repository;
        _combinedHandler = combinedHandler;
    }

    // 1. Solo la lista de Grupos (Ideal para el primer Select)
    [HttpGet("groups")]
    public async Task<ActionResult<List<GroupFilterDto>>> GetGroups()
    {
        var groups = await _repository.GetGroupsAsync();
        return Ok(groups);
    }

    // 2. Solo la lista de Países (Ideal para filtrar por Grupo en el Front)
    [HttpGet("list")]
    public async Task<ActionResult<List<CountryFilterDto>>> GetCountries()
    {
        var countries = await _repository.GetCountriesAsync();
        return Ok(countries);
    }

    // 3. La info combinada (por si quieres cargar todo al inicio)
    [HttpGet("info")]
    public async Task<IActionResult> GetCombinedInfo()
    {
        var response = await _combinedHandler.Handle();
        return Ok(response);
    }
}
