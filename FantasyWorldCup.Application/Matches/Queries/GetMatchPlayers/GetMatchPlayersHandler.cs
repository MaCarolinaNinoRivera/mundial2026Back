using FantasyWorldCup.Application.Matches.Interfaces;

namespace FantasyWorldCup.Application.Matches.Queries.GetMatchPlayers;

public class GetMatchPlayersHandler
{
    private readonly IPointsRepository _repository;

    public GetMatchPlayersHandler(IPointsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MatchPlayersDto>> Handle(Guid matchId)
    {
        var players = await _repository.GetPlayersByMatchCountriesAsync(matchId);

        if (players == null || !players.Any())
            throw new KeyNotFoundException("No se encontraron jugadores para este partido.");

        return players;
    }
}
