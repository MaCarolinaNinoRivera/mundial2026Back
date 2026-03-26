using FantasyWorldCup.Application.Auth.Interfaces;

namespace FantasyWorldCup.Application.Auth.UseCases.Logout;

public class LogoutHandler
{
    private readonly ITokenRepository _tokenRepo;

    public LogoutHandler(ITokenRepository tokenRepo)
    {
        _tokenRepo = tokenRepo;
    }

    public async Task Handle(string refreshToken)
    {
        var tokenRecord = await _tokenRepo.GetByTokenAsync(refreshToken);

        if (tokenRecord != null)
        {
            tokenRecord.Revoked = true;
            await _tokenRepo.UpdateAsync(tokenRecord);
            await _tokenRepo.SaveChangesAsync();
        }
    }
}
