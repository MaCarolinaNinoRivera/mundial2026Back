using FantasyWorldCup.Domain.Matches.Entities;
using FantasyWorldCup.Domain.Scoring.Entities;
using FantasyWorldCup.Domain.Teams.Entities;
using FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard;
using FantasyWorldCup.Application.Matches.Queries.GetMatchPlayers;

namespace FantasyWorldCup.Application.Matches.Interfaces;

public interface IPointsRepository
{
    Task<Match?> GetMatchByIdAsync(Guid matchId);
    void AddPlayerMatchStat(PlayerMatchStat stat);
    Task<List<PlayerMatchStat>> GetMatchStatsAsync(Guid matchId);
    Task<List<UserTeamPlayer>> GetStartersByPlayerIdAsync(Guid footballPlayerId);
    Task<List<LeaderboardDto>> GetGlobalLeaderboardAsync();
    Task<bool> ExistsLedgerEntryAsync(Guid sourceId, string sourceType);
    Task<bool> AreMatchPointsDistributedAsync(Guid matchId);
    void AddPointLedger(UserPointsLedger ledger);
    Task<List<MatchEvent>> GetMatchEventsAsync(Guid matchId);
    Task<UserMatchSubstitution?> GetSubstitutionByLineupIdAsync(Guid lineupId);
    Task<List<UserMatchLineup>> GetAllLineupsForMatchAsync(Guid matchId);
    Task<PlayerMatchStat?> GetStatByPlayerAndMatchAsync(Guid playerId, Guid matchId);
    void AddMatchEvent(MatchEvent matchEvent);
    Task<bool> PlayerExistsAsync(Guid playerId);
    Task<List<MatchPlayersDto>> GetPlayersByMatchCountriesAsync(Guid matchId);
    Task<IEnumerable<UserMatchLineup>> GetActiveLineupsForPlayerAsync(Guid playerId, Guid matchId);

    // Para guardar la sustitución que "cortará" los puntos
    void AddUserSubstitution(UserMatchSubstitution substitution);
    Task SaveChangesAsync();
}
