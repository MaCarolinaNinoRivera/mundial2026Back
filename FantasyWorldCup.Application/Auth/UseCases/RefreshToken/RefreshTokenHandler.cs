using FantasyWorldCup.Application.Auth.Interfaces;
using FantasyWorldCup.Application.Auth.UseCases.Login;
using FantasyWorldCup.Domain.Auth.Entities;
using RefreshTokenEntity = FantasyWorldCup.Domain.Auth.Entities.RefreshToken;

namespace FantasyWorldCup.Application.Auth.UseCases.RefreshToken;

public class RefreshTokenHandler
{
    private readonly IUserRepository _userRepo;
    private readonly ITokenRepository _tokenRepo;
    private readonly IJwtProvider _jwtProvider;

    public RefreshTokenHandler(
        IUserRepository userRepo,
        ITokenRepository tokenRepo,
        IJwtProvider jwtProvider)
    {
        _userRepo = userRepo;
        _tokenRepo = tokenRepo;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResponse> Handle(string refreshToken)
    {
        var tokenRecord = await _tokenRepo.GetByTokenAsync(refreshToken);

        if (tokenRecord is null)
        {
            throw new UnauthorizedAccessException("The provided Refresh Token does not exist.");
        }

        if (tokenRecord.Revoked || tokenRecord.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("The token has already been used or has expired.");
        }

        var user = await _userRepo.GetByIdAsync(tokenRecord.UserId);

        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        tokenRecord.Revoked = true;
        await _tokenRepo.UpdateAsync(tokenRecord);

        var newAccess = _jwtProvider.GenerateAccessToken(user);
        var newRefresh = _jwtProvider.GenerateRefreshToken();

        await _tokenRepo.AddAsync(new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newRefresh,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false,
            CreatedAt = DateTime.UtcNow
        });

        await _tokenRepo.SaveChangesAsync();
        return new AuthResponse(newAccess, newRefresh);
    }
}
