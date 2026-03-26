using Microsoft.EntityFrameworkCore;
using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Domain.Matches.Entities;
using FantasyWorldCup.Application.Matches.Queries.GetMatches;

namespace FantasyWorldCup.Infrastructure.Persistence.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly AppDbContext _context;

    public MatchRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Match match)
    {
        await _context.Matches.AddAsync(match);
    }

    public async Task<Match?> GetByIdAsync(Guid id)
    {
        return await _context.Matches.FindAsync(id);
    }

    public async Task<MatchDto?> GetByIdWithNamesAsync(Guid id)
    {
        return await _context.Matches
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new MatchDto(
                m.Id,
                _context.Countries
                    .Where(c => c.Id == m.HomeCountryId)
                    .Select(c => c.Name)
                    .FirstOrDefault() ?? "unknown",

                _context.Countries
                    .Where(c => c.Id == m.AwayCountryId)
                    .Select(c => c.Name)
                    .FirstOrDefault() ?? "unknown",

                m.StartTime,
                m.HomeGoals,
                m.AwayGoals,
                m.Status.ToString()
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<MatchDto>> GetAllWithNamesAsync()
    {
        return await _context.Matches
            .AsNoTracking()
            .Select(m => new MatchDto(
                m.Id,
                _context.Countries.Where(c => c.Id == m.HomeCountryId).Select(c => c.Name).FirstOrDefault() ?? "unknown",
                _context.Countries.Where(c => c.Id == m.AwayCountryId).Select(c => c.Name).FirstOrDefault() ?? "unknown",
                m.StartTime,
                m.HomeGoals,
                m.AwayGoals,
                m.Status.ToString()
            ))
            .ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetAllAsync()
    {
        return await _context.Matches.ToListAsync();
    }

    public void Remove(Match match)
    {
        _context.Matches.Remove(match);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PlayerMatchStat>> GetPlayerStatsByMatchIdAsync(Guid matchId)
    {
        return await _context.Set<PlayerMatchStat>()
            .Include(s => s.Player)
            .Where(s => s.MatchId == matchId)
            .ToListAsync();
    }

    public async Task<List<Match>> GetUpcomingMatchesAsync(DateTime startDate, int daysAhead)
    {
        var endDate = startDate.AddDays(daysAhead);

        return await _context.Matches
            .Include(m => m.HomeCountry)
            .Include(m => m.AwayCountry)
            .Where(m => m.StartTime >= startDate && m.StartTime <= endDate)
            .OrderBy(m => m.StartTime)
            .ToListAsync();
    }

    public async Task<List<Match>> GetAllMatchResultsAsync()
    {
        return await _context.Matches
            .Include(m => m.HomeCountry)
                .ThenInclude(c => c.Group) // JOIN groups g
            .Include(m => m.AwayCountry)
                .ThenInclude(c => c.Group) // JOIN groups g2
            .OrderByDescending(m => m.StartTime) // Los más recientes primero
            .ToListAsync();
    }

    public async Task<List<Match>> GetMatchResultsByDateAsync(DateTime? date)
    {
        var query = _context.Matches
            .Include(m => m.HomeCountry).ThenInclude(c => c.Group)
            .Include(m => m.AwayCountry).ThenInclude(c => c.Group)
            .AsQueryable();

        if (date.HasValue)
        {
            var utcDate = DateTime.SpecifyKind(date.Value.Date, DateTimeKind.Utc);
            var dayEnd = utcDate.AddDays(1);

            query = query.Where(m => m.StartTime >= utcDate && m.StartTime < dayEnd);
        }

        return await query
            .OrderByDescending(m => m.StartTime)
            .ToListAsync();
    }
}