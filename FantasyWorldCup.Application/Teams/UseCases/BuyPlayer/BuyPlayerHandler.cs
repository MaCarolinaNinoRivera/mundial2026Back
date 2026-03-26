using FantasyWorldCup.Application.Teams.Interfaces; // <-- Ahora usa la interfaz
using FantasyWorldCup.Application.Teams.Services;
using FantasyWorldCup.Domain.Teams.Entities;

namespace FantasyWorldCup.Application.Teams.UseCases.BuyPlayer;

public record BuyPlayerCommand(Guid PlayerId);

public class BuyPlayerHandler
{
    private readonly ITeamRepository _repository; // <-- CAMBIADO
    private readonly TeamValidator _validator;

    public BuyPlayerHandler(ITeamRepository repository, TeamValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task Handle(Guid userId, BuyPlayerCommand command)
    {
        var team = await _repository.GetByUserIdAsync(userId);

        // Cambiamos Exception por InvalidOperationException
        if (team == null || team.Locked)
            throw new InvalidOperationException("Equipo no encontrado o mercado cerrado.");

        var player = await _repository.GetPlayerByIdAsync(command.PlayerId);
        if (player == null)
            throw new KeyNotFoundException("Jugador no encontrado."); // Esto devolverá 404 según tu middleware

        var currentPlayers = team.TeamPlayers.Select(tp => tp.Player).ToList();

        var validation = _validator.CanAddPlayer(team, player, currentPlayers);
        if (!validation.Success)
            // Cambiamos Exception por InvalidOperationException
            throw new InvalidOperationException(validation.Message);

        team.AvailableBudget -= player.BasePrice;

        var newTeamPlayer = new UserTeamPlayer
        {
            Id = Guid.NewGuid(),
            UserTeamId = team.Id,
            FootballPlayerId = player.Id,
            IsStarter = false
        };

        await _repository.AddPlayerToTeamAsync(newTeamPlayer);
        await _repository.SaveChangesAsync();
    }
}
