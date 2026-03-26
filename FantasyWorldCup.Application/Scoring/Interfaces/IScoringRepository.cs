using FantasyWorldCup.Domain.Scoring.Entities;
using FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard;

namespace FantasyWorldCup.Application.Scoring.Interfaces;

public interface IScoringRepository
{
    Task<List<ScoringRule>> GetActiveRulesAsync();
    Task<IEnumerable<ScoringRule>> GetAllRulesAsync();
    Task<List<LeaderboardDto>> GetLeaderboardAsync();
    Task SaveRankHistoryAsync(List<UserRankHistory> historyRecords);
    Task<List<UserBadgesCountDto>> GetBadgesLeaderboardAsync();
    Task<List<LeaderboardDto>> GetGlobalLeaderboardAsync();
    Task<List<UserRankHistory>> GetLatestRankHistoryAsync();
    Task<List<UserRankHistory>> GetPreviousRankHistoryAsync();
}
