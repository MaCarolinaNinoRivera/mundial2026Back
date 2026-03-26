using FantasyWorldCup.Application.Countries.Interfaces;

namespace FantasyWorldCup.Application.Countries.Queries.GetFilters;

public class GetFilteredPlayersHandler
{
    private readonly ICountryRepository _repository;

    public GetFilteredPlayersHandler(ICountryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PlayerListDto>> Handle(Guid? countryId, Guid? groupId, string? position)
    {
        // Llamamos al repositorio que ya tiene toda la lógica de filtrado
        return await _repository.GetFilteredPlayersAsync(countryId, groupId, position);
    }
}
