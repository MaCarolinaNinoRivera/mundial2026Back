using FantasyWorldCup.Application.Countries.Interfaces;
using FantasyWorldCup.Application.Countries.Queries.GetFilters;

namespace FantasyWorldCup.Application.Countries.Queries.GetCountriesAndGroups;

public class GetCountriesAndGroupsHandler
{
    private readonly ICountryRepository _repository;

    public GetCountriesAndGroupsHandler(ICountryRepository repository)
    {
        _repository = repository;
    }

    public async Task<CountriesAndGroupsResponse> Handle()
    {
        // Ahora las listas coinciden perfectamente con lo que devuelve el repositorio
        return new CountriesAndGroupsResponse
        {
            Groups = await _repository.GetGroupsAsync(),
            Countries = await _repository.GetCountriesAsync()
        };
    }
}
