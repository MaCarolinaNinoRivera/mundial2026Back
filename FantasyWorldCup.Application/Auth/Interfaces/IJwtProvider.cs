using FantasyWorldCup.Domain.Auth.Entities;

namespace FantasyWorldCup.Application.Auth.Interfaces;

public interface IJwtProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
