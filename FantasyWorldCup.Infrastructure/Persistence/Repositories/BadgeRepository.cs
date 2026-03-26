using FantasyWorldCup.Application.Badges.Interfaces;
using FantasyWorldCup.Domain.Badges.Entities;
using FantasyWorldCup.Domain.Scoring.Entities;
using FantasyWorldCup.Application.Badges.Queries.GetBadges;
using FantasyWorldCup.Application.Countries.Queries.GetFilters; // <--- AGREGA ESTA LÍNEA
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;

namespace FantasyWorldCup.Infrastructure.Persistence.Repositories;

public class BadgeRepository : IBadgeRepository
{
    private readonly AppDbContext _context;

    public BadgeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Badge?> GetBadgeByCriteriaAsync(string criteriaType)
    {
        return await _context.Set<Badge>()
            .FirstOrDefaultAsync(b => b.CriteriaType == criteriaType);
    }

    public async Task<bool> UserHasBadgeAsync(Guid userId, Guid badgeId)
    {
        return await _context.Set<UserBadge>()
            .AnyAsync(ub => ub.UserId == userId && ub.BadgeId == badgeId);
    }

    public async Task AwardBadgeAsync(UserBadge userBadge, UserPointsLedger? pointEntry)
    {
        // Usamos una transacción para que si falla el punto, no se guarde la medalla (y viceversa)
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Set<UserBadge>().AddAsync(userBadge);

            if (pointEntry != null)
            {
                await _context.Set<UserPointsLedger>().AddAsync(pointEntry);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<BadgeDto>> GetAllBadgesAsync()
    {
        return await _context.Badges
            .Select(b => new BadgeDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                IconUrl = b.IconUrl,
                CriteriaType = b.CriteriaType
            })
            .ToListAsync();
    }

    public async Task<List<PlayerListDto>> GetFilteredPlayersAsync(Guid? countryId, Guid? groupId, string? position)
    {
        var query = _context.FootballPlayers
            .Include(p => p.Country)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (countryId.HasValue) query = query.Where(p => p.CountryId == countryId);
        if (groupId.HasValue) query = query.Where(p => p.Country.GroupId == groupId);
        if (!string.IsNullOrEmpty(position)) query = query.Where(p => p.Position == position);

        return await query
            .Select(p => new PlayerListDto // <-- Usamos inicialización con llaves
            {
                Id = p.Id,
                Name = p.Name,
                Position = p.Position,
                BasePrice = p.BasePrice,
                CountryName = p.Country.Name,
                GroupId = p.Country.GroupId
            })
            .ToListAsync();
    }
}
