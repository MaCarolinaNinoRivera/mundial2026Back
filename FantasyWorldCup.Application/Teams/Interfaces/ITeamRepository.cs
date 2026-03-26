using FantasyWorldCup.Domain.Teams.Entities;
using FantasyWorldCup.Domain.Countries.Entities;

namespace FantasyWorldCup.Application.Teams.Interfaces;

public interface ITeamRepository
{
    Task<UserTeam?> GetTeamByUserIdAsync(Guid userId);
    Task<UserTeam?> GetByUserIdAsync(Guid userId);
    Task<FootballPlayer?> GetPlayerByIdAsync(Guid playerId);
    Task<UserTeamPlayer?> GetPlayerInTeamAsync(Guid teamId, Guid footballPlayerId);
    Task AddPlayerToTeamAsync(UserTeamPlayer teamPlayer);
    Task AddTeamAsync(UserTeam team);
    Task LockAllTeamsAsync();
    void RemovePlayerFromTeam(UserTeamPlayer playerInTeam);
    Task<List<UserTeamPlayer>> GetTeamPlayersAsync(Guid userTeamId);
    Task ClearAndSetLineupAsync(Guid userTeamId, Guid matchId, List<Guid> playerIds);
    Task<int> GetSubstitutionCountAsync(Guid userId, Guid matchId);
    Task ExecuteSubstitutionAsync(Guid userId, Guid matchId, Guid playerOutId, Guid playerInId);
    Task<List<UserMatchSubstitution>> GetMatchSubstitutionsAsync(Guid userTeamId, Guid matchId);
    Task<UserTeam?> GetFullTeamByUserIdAsync(Guid userId);
    Task SwapLineupPlayersBeforeMatchAsync(Guid userTeamId, Guid matchId, Guid playerOutId, Guid playerInId);
    Task<List<UserTeamPlayer>> GetAvailableBenchAsync(Guid userTeamId, Guid matchId);
    Task<List<UserMatchLineup>> GetMatchLineupAsync(Guid matchId, Guid userId);
    Task SaveChangesAsync();
}
