using FantasyWorldCup.Application.Auth.Interfaces;
using FantasyWorldCup.Domain.Auth.Entities;
using FantasyWorldCup.Domain.Auth.ValueObjects;

using RefreshTokenEntity = FantasyWorldCup.Domain.Auth.Entities.RefreshToken;

namespace FantasyWorldCup.Application.Auth.UseCases.Login;

public class LoginHandler
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtProvider _jwt;
    private readonly ITokenRepository _tokenRepo;

    public LoginHandler(
        IUserRepository repo,
        IPasswordHasher hasher,
        IJwtProvider jwt,
        ITokenRepository tokenRepo)
    {
        _repo = repo;
        _hasher = hasher;
        _jwt = jwt;
        _tokenRepo = tokenRepo;
    }

    public async Task<AuthResponse> Handle(LoginCommand command)
    {
        if (!command.Email.Contains("@"))
            throw new ArgumentException("Invalid email format");

        var email = new Email(command.Email);
        var user = await _repo.GetByEmail(email);

        if (user is null || !_hasher.Verify(command.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        if (user is null)
            throw new UnauthorizedAccessException();

        if (!_hasher.Verify(command.Password, user.PasswordHash))
            throw new UnauthorizedAccessException();

        var access = _jwt.GenerateAccessToken(user);
        var refresh = _jwt.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refresh,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _tokenRepo.AddAsync(refreshTokenEntity);
        await _tokenRepo.SaveChangesAsync();

        return new AuthResponse(access, refresh);
    }
}
