using FantasyWorldCup.Domain.Countries.Entities;
using FantasyWorldCup.Application.Countries.Queries.GetFilters;

namespace FantasyWorldCup.Application.Countries.Interfaces;

public interface ICountryRepository
{
    Task<bool> ExistsAsync(Guid id);
    Task<FootballPlayer?> GetPlayerByIdAsync(Guid playerId);
    Task<List<CountryFilterDto>> GetCountriesAsync();
    Task<List<GroupFilterDto>> GetGroupsAsync();
    Task<List<PlayerListDto>> GetFilteredPlayersAsync(Guid? countryId, Guid? groupId, string? position);
}
