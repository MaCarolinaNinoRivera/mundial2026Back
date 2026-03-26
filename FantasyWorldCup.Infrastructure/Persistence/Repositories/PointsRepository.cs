using Microsoft.EntityFrameworkCore;
using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Domain.Matches.Entities;
using FantasyWorldCup.Domain.Scoring.Entities;
using FantasyWorldCup.Domain.Teams.Entities;
using FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard;
using FantasyWorldCup.Application.Matches.Queries.GetMatchPlayers;
using FantasyWorldCup.Domain.Auth.Enums;

namespace FantasyWorldCup.Infrastructure.Persistence.Repositories;

public class PointsRepository : IPointsRepository
{
    private readonly AppDbContext _context;
    public PointsRepository(AppDbContext context) => _context = context;

    public async Task<Match?> GetMatchByIdAsync(Guid matchId)
    {
        return await _context.Matches.FindAsync(matchId);
    }

    public void AddPlayerMatchStat(PlayerMatchStat stat)
    {
        _context.PlayerMatchStats.Add(stat);
    }

    public async Task<List<PlayerMatchStat>> GetMatchStatsAsync(Guid matchId)
    {
        return await _context.PlayerMatchStats
            .Include(s => s.Player)
            .Where(s => s.MatchId == matchId)
            .ToListAsync();
    }

    public async Task<List<UserTeamPlayer>> GetStartersByPlayerIdAsync(Guid footballPlayerId)
    {
        return await _context.UserTeamPlayers
            .Include(utp => utp.UserTeam)
            // Asegúrate de que aquí diga FootballPlayerId o PlayerId según tu tabla user_team_players
            .Where(utp => utp.FootballPlayerId == footballPlayerId && utp.IsStarter)
            .ToListAsync();
    }

    public void AddPointLedger(UserPointsLedger ledger)
    {
        _context.Set<UserPointsLedger>().Add(ledger);
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task<List<LeaderboardDto>> GetGlobalLeaderboardAsync()
    {
        // 1. Obtener la fecha del snapshot de HOY y de AYER
        // (Equivalente a tus CTEs 'today' y 'yesterday')
        var todayRecords = await _context.UserRankHistories
            .Where(h => h.CalculatedAt.Date == DateTime.UtcNow.Date)
            .ToListAsync();

        var yesterdayRecords = await _context.UserRankHistories
            .Where(h => h.CalculatedAt.Date == DateTime.UtcNow.Date.AddDays(-1))
            .ToListAsync();

        // 2. Traer los usuarios para tener el Alias y Username
        var users = await _context.Users.Where(u => u.Role != UserRole.Admin).ToListAsync();

        // 3. Cruzar los datos (Simulando el LEFT JOIN de tu SQL)
        var leaderboard = todayRecords.Select(t =>
        {
            var y = yesterdayRecords.FirstOrDefault(prev => prev.UserId == t.UserId);
            var u = users.FirstOrDefault(user => user.Id == t.UserId);

            return new LeaderboardDto
            {
                UserId = t.UserId,
                Username = u?.Username ?? "Desconocido",
                Alias = u?.Alias ?? "Sin Alias",
                TotalPoints = t.TotalPoints, // Puntos de hoy
                Position = t.RankPosition,   // Posición de hoy

                // Aquí guardamos el cálculo del cambio (t.rank_position - y.rank_position)
                // Puedes agregar una propiedad 'RankChange' a tu DTO si quieres mostrar el -1 o +2
                RankChange = t.RankPosition - (y?.RankPosition ?? 0)
            };
        })
        .OrderBy(x => x.Position)
        .ToList();

        return leaderboard;
    }

    public async Task<bool> ExistsLedgerEntryAsync(Guid sourceId, string sourceType)
    {
        return await _context.Set<UserPointsLedger>()
            .AnyAsync(l => l.SourceId == sourceId && l.SourceType == sourceType);
    }

    public async Task<bool> AreMatchPointsDistributedAsync(Guid matchId)
    {
        return await _context.Set<UserPointsLedger>()
            .Where(l => l.SourceType == "PLAYER_PERFORMANCE")
            .Join(_context.PlayerMatchStats.Where(s => s.MatchId == matchId),
                  ledger => ledger.SourceId,
                  stat => stat.Id,
                  (ledger, stat) => ledger)
            .AnyAsync();
    }

    // Obtiene todos los goles, asistencias y tarjetas con su minuto exacto
    public async Task<List<MatchEvent>> GetMatchEventsAsync(Guid matchId)
    {
        return await _context.MatchEvents
            .Include(e => e.Player)
            .Where(e => e.MatchId == matchId)
            .OrderBy(e => e.Minute) // Ordenados cronológicamente
            .ToListAsync();
    }

    // Busca si un jugador específico de una alineación fue parte de un cambio
    public async Task<UserMatchSubstitution?> GetSubstitutionByLineupIdAsync(Guid lineupId)
    {
        // Buscamos si el lineupId proporcionado fue el que SALIÓ o el que ENTRÓ
        // Esto es vital para saber en qué minuto se activó o desactivó el jugador para el usuario
        return await _context.UserMatchSubstitutions
            .FirstOrDefaultAsync(s => s.LineupPlayerOutId == lineupId || s.LineupPlayerInId == lineupId);
    }

    // Método extra que necesitarás en el Handler para obtener todas las alineaciones de un partido
    public async Task<List<UserMatchLineup>> GetAllLineupsForMatchAsync(Guid matchId)
    {
        return await _context.UserMatchLineups
            .Include(l => l.UserTeam)
            .Include(l => l.UserTeamPlayer)
                .ThenInclude(utp => utp.Player)
            .Where(l => l.MatchId == matchId)
            .ToListAsync();
    }

    // Método para obtener la estadística general (útil para los minutos jugados)
    public async Task<PlayerMatchStat?> GetStatByPlayerAndMatchAsync(Guid playerId, Guid matchId)
    {
        return await _context.PlayerMatchStats
            .FirstOrDefaultAsync(s => s.PlayerId == playerId && s.MatchId == matchId);
    }

    public void AddMatchEvent(MatchEvent matchEvent)
    {
        _context.MatchEvents.Add(matchEvent);
    }

    public async Task<bool> PlayerExistsAsync(Guid playerId)
    {
        return await _context.FootballPlayers.AnyAsync(p => p.Id == playerId);
    }

    public async Task<List<MatchPlayersDto>> GetPlayersByMatchCountriesAsync(Guid matchId)
    {
        var match = await _context.Matches.FindAsync(matchId);
        if (match == null) return new List<MatchPlayersDto>();

        return await _context.FootballPlayers
            .Include(p => p.Country)
            .Where(p => p.CountryId == match.HomeCountryId || p.CountryId == match.AwayCountryId)
            .Select(p => new MatchPlayersDto
            {
                PlayerId = p.Id,
                Name = p.Name,
                Position = p.Position,
                CountryName = p.Country.Name,
                IsHomeTeam = p.CountryId == match.HomeCountryId
            })
            .OrderBy(p => !p.IsHomeTeam)
            .ThenBy(p => p.Position)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserMatchLineup>> GetActiveLineupsForPlayerAsync(Guid playerId, Guid matchId)
    {
        return await _context.UserMatchLineups
            .Include(l => l.UserTeamPlayer)
            .Where(l => l.MatchId == matchId &&
                        l.UserTeamPlayer.FootballPlayerId == playerId &&
                        l.IsStarter == true)
            .ToListAsync();
    }

    public void AddUserSubstitution(UserMatchSubstitution substitution)
    {
        _context.UserMatchSubstitutions.Add(substitution);
    }
}
