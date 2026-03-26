using FantasyWorldCup.Application.Auth.Interfaces;

namespace FantasyWorldCup.Infrastructure.Security.Hashing;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
