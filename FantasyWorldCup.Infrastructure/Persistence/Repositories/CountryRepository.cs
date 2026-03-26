using FantasyWorldCup.Application.Countries.Interfaces;
using FantasyWorldCup.Domain.Countries.Entities;
using FantasyWorldCup.Infrastructure.Persistence;
using FantasyWorldCup.Application.Countries.Queries.GetFilters;
using Microsoft.EntityFrameworkCore;

namespace FantasyWorldCup.Infrastructure.Persistence.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly AppDbContext _context;

    public CountryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Countries.AnyAsync(c => c.Id == id);
    }

    public async Task<FootballPlayer?> GetPlayerByIdAsync(Guid playerId)
    {
        // Buscamos en la tabla de jugadores usando el ID
        return await _context.FootballPlayers
            .FirstOrDefaultAsync(p => p.Id == playerId);
    }

    public async Task<List<CountryFilterDto>> GetCountriesAsync()
    {
        return await _context.Countries
            .Select(c => new CountryFilterDto
            {
                Id = c.Id,
                Name = c.Name,
                GroupId = c.GroupId
            })
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<GroupFilterDto>> GetGroupsAsync()
    {
        return await _context.Groups
            .Select(g => new GroupFilterDto
            {
                Id = g.Id,
                Name = g.Name
            })
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    public async Task<List<PlayerListDto>> GetFilteredPlayersAsync(Guid? countryId, Guid? groupId, string? position)
    {
        var query = _context.FootballPlayers
            .Include(p => p.Country)
            .Where(p => p.IsActive)
            .AsQueryable();

        // Filtro por País
        if (countryId.HasValue)
            query = query.Where(p => p.CountryId == countryId);

        // Filtro por Grupo (a través de la tabla countries)
        if (groupId.HasValue)
            query = query.Where(p => p.Country.GroupId == groupId);

        // Filtro por Posición (Forward, Midfielder, etc.)
        if (!string.IsNullOrEmpty(position))
            query = query.Where(p => p.Position == position);

        return await query
            .Select(p => new PlayerListDto
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
