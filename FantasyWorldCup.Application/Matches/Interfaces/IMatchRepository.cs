using FantasyWorldCup.Domain.Matches.Entities;
using FantasyWorldCup.Application.Matches.Queries.GetMatches;

namespace FantasyWorldCup.Application.Matches.Interfaces;

public interface IMatchRepository
{
    Task AddAsync(Match match);
    Task<Match?> GetByIdAsync(Guid id);
    Task<IEnumerable<MatchDto>> GetAllWithNamesAsync();
    Task<IEnumerable<Match>> GetAllAsync();
    void Remove(Match match);
    Task SaveChangesAsync();
    Task<MatchDto?> GetByIdWithNamesAsync(Guid id);
    Task<IEnumerable<PlayerMatchStat>> GetPlayerStatsByMatchIdAsync(Guid matchId);
    Task<List<Match>> GetUpcomingMatchesAsync(DateTime date, int daysAhead);
    Task<List<Match>> GetAllMatchResultsAsync();
    Task<List<Match>> GetMatchResultsByDateAsync(DateTime? date);
}
