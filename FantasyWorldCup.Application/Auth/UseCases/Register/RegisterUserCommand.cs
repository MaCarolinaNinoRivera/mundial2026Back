using FantasyWorldCup.Domain.Auth.Enums;

namespace FantasyWorldCup.Application.Auth.UseCases.Register;

public record RegisterUserCommand(
    string Email,
    string Password,
    string Username,
    string Alias,
    UserRole Role
);
