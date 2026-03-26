using FantasyWorldCup.Application.Countries.Queries.GetFilters;

namespace FantasyWorldCup.Application.Countries.Queries.GetCountriesAndGroups;

public class CountriesAndGroupsResponse
{
    // Usamos explícitamente los DTOs de la carpeta GetFilters
    public List<GroupFilterDto> Groups { get; set; } = new();
    public List<CountryFilterDto> Countries { get; set; } = new();
}
