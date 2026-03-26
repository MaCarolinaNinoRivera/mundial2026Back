using FantasyWorldCup.Domain.Common;
using FantasyWorldCup.Domain.Auth.Enums;
using FantasyWorldCup.Domain.Auth.ValueObjects;

namespace FantasyWorldCup.Domain.Auth.Entities;

public class User : BaseEntity<Guid>
{
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public string Username { get; private set; }
    public string Alias { get; private set; }
    public bool IsActive { get; private set; }
    public int TotalVotes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? AvatarUrl { get; private set; }

    // CAMBIO AQUÍ: Inicializa los campos para callar la advertencia
    private User()
    {
        Email = null!;
        PasswordHash = string.Empty;
        Username = string.Empty;
        Alias = string.Empty;
    }

    public User(
        Email email,
        string username,
        string alias,
        string passwordHash,
        UserRole role)
    {
        Id = Guid.NewGuid();
        Email = email;
        Username = username;
        Alias = alias;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
        TotalVotes = 0;
        CreatedAt = DateTime.UtcNow;
    }
}
