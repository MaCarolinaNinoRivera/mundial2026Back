using FantasyWorldCup.Domain.Auth.Entities;
using FantasyWorldCup.Domain.Auth.ValueObjects;

namespace FantasyWorldCup.Application.Auth.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmail(Email email);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> ExistsByEmail(Email email);
    Task Add(User user);
    Task<List<User>> GetAllActivePlayersAsync();
}
