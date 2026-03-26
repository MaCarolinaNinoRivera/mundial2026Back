using FantasyWorldCup.Application.Teams.Interfaces;
using FantasyWorldCup.Application.Countries.Interfaces;

namespace FantasyWorldCup.Application.Teams.UseCases.SellPlayer;

public class SellPlayerHandler
{
    private readonly ITeamRepository _teamRepo;
    private readonly ICountryRepository _countryRepo; // Para saber el precio del jugador

    public SellPlayerHandler(ITeamRepository teamRepo, ICountryRepository countryRepo)
    {
        _teamRepo = teamRepo;
        _countryRepo = countryRepo;
    }

    public async Task Handle(SellPlayerCommand command)
    {
        var team = await _teamRepo.GetTeamByUserIdAsync(command.UserId);

        if (team == null || team.Locked)
            throw new Exception("Equipo no encontrado o mercado cerrado.");

        var playerInTeam = await _teamRepo.GetPlayerInTeamAsync(team.Id, command.FootballPlayerId);
        if (playerInTeam == null) throw new Exception("El jugador no está en tu equipo.");

        // Obtenemos el precio del jugador para devolver el dinero
        var playerInfo = await _countryRepo.GetPlayerByIdAsync(command.FootballPlayerId);
        if (playerInfo == null) throw new Exception("Información del jugador no encontrada.");

        // 1. Devolver el dinero al presupuesto
        team.AvailableBudget += playerInfo.BasePrice;

        // 2. Eliminar de la lista de jugadores del equipo
        _teamRepo.RemovePlayerFromTeam(playerInTeam);

        await _teamRepo.SaveChangesAsync();
    }
}
