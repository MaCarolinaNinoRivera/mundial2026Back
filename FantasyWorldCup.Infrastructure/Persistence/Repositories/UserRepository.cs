using Microsoft.EntityFrameworkCore;
using FantasyWorldCup.Application.Auth.Interfaces;
using FantasyWorldCup.Domain.Auth.Entities;
using FantasyWorldCup.Domain.Auth.ValueObjects;
using FantasyWorldCup.Domain.Auth.Enums;

namespace FantasyWorldCup.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmail(Email email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<bool> ExistsByEmail(Email email)
    {
        return await _context.Users
            .AnyAsync(x => x.Email == email);
    }

    public async Task Add(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<List<User>> GetAllActivePlayersAsync()
    {
        return await _context.Users
            .Where(u => u.Role != UserRole.Admin && u.IsActive)
            .ToListAsync();
    }
}
