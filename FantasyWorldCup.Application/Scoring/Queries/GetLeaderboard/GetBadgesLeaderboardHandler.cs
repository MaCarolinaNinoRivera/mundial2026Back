using FantasyWorldCup.Application.Scoring.Interfaces;

namespace FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard;

public class GetBadgesLeaderboardHandler // <-- Nombre de la clase
{
    private readonly IScoringRepository _repository;

    // EL NOMBRE DEBE COINCIDIR CON LA CLASE:
    public GetBadgesLeaderboardHandler(IScoringRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<UserBadgesCountDto>> Handle()
    {
        return await _repository.GetBadgesLeaderboardAsync();
    }
}
