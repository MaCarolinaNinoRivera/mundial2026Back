using FantasyWorldCup.Domain.Auth.Entities;

namespace FantasyWorldCup.Application.Auth.Interfaces;

public interface ITokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task RevokeAllUserTokensAsync(Guid userId);
    Task SaveChangesAsync();
}
