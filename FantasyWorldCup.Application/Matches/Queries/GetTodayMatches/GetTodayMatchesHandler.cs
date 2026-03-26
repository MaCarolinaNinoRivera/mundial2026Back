using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Application.Matches.Queries.GetMatches;

namespace FantasyWorldCup.Application.Matches.Queries.GetTodayMatches;

public class GetTodayMatchesHandler
{
    private readonly IMatchRepository _repository;

    public GetTodayMatchesHandler(IMatchRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MatchDto>> Handle()
    {
        // Empezamos desde el inicio del día actual
        var startFrom = DateTime.UtcNow.Date;

        // Traemos los partidos de los próximos 5 días
        var matches = await _repository.GetUpcomingMatchesAsync(startFrom, 5);

        return matches.Select(m => new MatchDto(
            m.Id,
            m.HomeCountry?.Name ?? "TBD",
            m.AwayCountry?.Name ?? "TBD",
            m.StartTime,
            m.HomeGoals,
            m.AwayGoals,
            m.Status.ToString()
        )).ToList();
    }
}
