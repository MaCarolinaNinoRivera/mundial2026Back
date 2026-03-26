using FantasyWorldCup.Application.Auth.Interfaces;
using FantasyWorldCup.Application.Teams.Interfaces; // <-- NUEVO
using FantasyWorldCup.Domain.Auth.Entities;
using FantasyWorldCup.Domain.Auth.ValueObjects;
using FantasyWorldCup.Domain.Auth.Enums;
using FantasyWorldCup.Domain.Teams.Entities; // <-- NUEVO

namespace FantasyWorldCup.Application.Auth.UseCases.Register;

public class RegisterUserHandler
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher _hasher;
    private readonly ITeamRepository _teamRepo; // <-- NUEVO

    public RegisterUserHandler(
        IUserRepository repo,
        IPasswordHasher hasher,
        ITeamRepository teamRepo) // <-- Inyectar repositorio de equipos
    {
        _repo = repo;
        _hasher = hasher;
        _teamRepo = teamRepo;
    }

    public async Task<Guid> Handle(RegisterUserCommand command)
    {
        var email = new Email(command.Email);

        if (await _repo.ExistsByEmail(email))
            throw new ApplicationException("Email already exists");

        var hash = _hasher.Hash(command.Password);

        var user = new User(
            email,
            command.Username,
            command.Alias,
            hash,
            command.Role
        );

        // 1. Guardar el usuario
        await _repo.Add(user);

        // 2. CREAR EL EQUIPO INICIAL PARA EL FANTASY
        var initialTeam = new UserTeam
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TotalBudget = 100000000,     // 100 Millones de presupuesto inicial
            AvailableBudget = 100000000, // Empieza con todo el dinero disponible
            Locked = false,              // Mercado abierto para él
            CreatedAt = DateTime.UtcNow
        };

        // Asumimos que tu ITeamRepository tiene un método para guardar el equipo
        // Si no lo tienes, puedes agregarlo a la interfaz e implementar en Infrastructure
        await _teamRepo.AddTeamAsync(initialTeam);
        await _teamRepo.SaveChangesAsync();

        return user.Id;
    }
}
