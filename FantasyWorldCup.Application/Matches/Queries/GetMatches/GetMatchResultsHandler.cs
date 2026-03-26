using FantasyWorldCup.Application.Matches.Interfaces;

namespace FantasyWorldCup.Application.Matches.Queries.GetMatches;

public class GetMatchResultsHandler
{
    private readonly IMatchRepository _repository;

    public GetMatchResultsHandler(IMatchRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MatchResultDto>> Handle(DateTime? date = null)
    {
        var matches = await _repository.GetMatchResultsByDateAsync(date);

        return matches.Select(m => new MatchResultDto
        {
            IdMatch = m.Id,
            GroupA = m.HomeCountry?.Group?.Name ?? "TBD",
            Local = m.HomeCountry?.Name ?? "Por definir",
            GroupB = m.AwayCountry?.Group?.Name ?? "TBD",
            Visitante = m.AwayCountry?.Name ?? "Por definir",
            StartTime = m.StartTime,
            HomeGoals = m.HomeGoals,
            AwayGoals = m.AwayGoals,
            Status = m.Status.ToString(),
        }).ToList();
    }
}
