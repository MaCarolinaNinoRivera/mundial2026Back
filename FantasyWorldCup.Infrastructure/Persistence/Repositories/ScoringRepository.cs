using FantasyWorldCup.Application.Scoring.Interfaces;
using FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard; // Para LeaderboardDto
using FantasyWorldCup.Domain.Scoring.Entities; // Para UserRankHistory y ScoringRule
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyWorldCup.Infrastructure.Persistence.Repositories;

public class ScoringRepository : IScoringRepository
{
    private readonly AppDbContext _context;

    public ScoringRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ScoringRule>> GetAllRulesAsync()
    {
        return await _context.Set<ScoringRule>().ToListAsync();
    }

    public async Task<List<ScoringRule>> GetActiveRulesAsync()
    {
        return await _context.Set<ScoringRule>()
            .OrderBy(r => r.RuleKey)
            .ToListAsync();
    }

    // Dentro de ScoringRepository.cs
    public async Task<List<LeaderboardDto>> GetLeaderboardAsync()
    {
        // En lugar de llamar a _context.Leaderboard (que no existe), 
        // llamamos al método que ya calcula todo manualmente:
        return await GetGlobalLeaderboardAsync();
    }

    public async Task SaveRankHistoryAsync(List<UserRankHistory> historyRecords)
    {
        await _context.Set<UserRankHistory>().AddRangeAsync(historyRecords);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserBadgesCountDto>> GetBadgesLeaderboardAsync()
    {
        // 1. Obtenemos los datos aplanados (Esto hace los JOINs automáticamente)
        var flatData = await _context.UserBadges
            .Select(ub => new
            {
                ub.UserId,
                Username = ub.User.Username,
                BadgeName = ub.Badge.Name,
                BadgeCriteria = ub.Badge.CriteriaType
            })
            .ToListAsync(); // Traemos a memoria para agrupar (más seguro para listas pequeñas/medianas)

        // 2. Agrupamos y mapeamos al DTO
        return flatData
            .GroupBy(x => new { x.UserId, x.Username })
            .Select(g => new UserBadgesCountDto
            {
                UserId = g.Key.UserId,
                Username = g.Key.Username,
                TotalBadges = g.Count(),
                Badges = g.Select(b => new BadgeDetailDto
                {
                    Name = b.BadgeName,
                    CriteriaType = b.BadgeCriteria
                }).ToList()
            })
            .OrderByDescending(x => x.TotalBadges)
            .ToList();
    }

    public async Task<List<LeaderboardDto>> GetGlobalLeaderboardAsync()
    {
        // 1. Sumamos puntos
        var pointsQuery = await _context.UserPointsLedger
            .GroupBy(l => l.UserId)
            .Select(g => new { UserId = g.Key, TotalPoints = g.Sum(p => p.Points) })
            .ToListAsync();

        // 2. Traemos usuarios (Asegúrate de tener el using de Domain.Auth.Entities)
        var users = await _context.Users
            .Where(u => u.IsActive)
            .ToListAsync();

        // 3. Cruzamos datos
        var leaderboard = users.Select(u => new LeaderboardDto
        {
            UserId = u.Id,
            Username = u.Username,
            Alias = u.Alias,
            TotalPoints = pointsQuery.FirstOrDefault(p => p.UserId == u.Id)?.TotalPoints ?? 0
        })
        .OrderByDescending(x => x.TotalPoints)
        .ToList();

        // 4. Posiciones
        for (int i = 0; i < leaderboard.Count; i++)
        {
            leaderboard[i].Position = i + 1;
        }

        return leaderboard;
    }

    public async Task<List<UserRankHistory>> GetLatestRankHistoryAsync()
    {
        var latestDate = await _context.UserRankHistories
            .MaxAsync(h => h.CalculatedAt);

        return await _context.UserRankHistories
            .Where(h => h.CalculatedAt == latestDate)
            .ToListAsync();
    }

    public async Task<List<UserRankHistory>> GetPreviousRankHistoryAsync()
    {
        // Obtenemos todas las fechas distintas de snapshots
        var dates = await _context.UserRankHistories
            .Select(h => h.CalculatedAt)
            .Distinct()
            .OrderByDescending(d => d)
            .Take(2)
            .ToListAsync();

        // Si hay menos de 2 snapshots, devolvemos una lista vacía para no romper el cálculo
        if (dates.Count < 2) return new List<UserRankHistory>();

        var previousDate = dates[1]; // La segunda fecha más reciente

        return await _context.UserRankHistories
            .Where(h => h.CalculatedAt == previousDate)
            .ToListAsync();
    }
}
