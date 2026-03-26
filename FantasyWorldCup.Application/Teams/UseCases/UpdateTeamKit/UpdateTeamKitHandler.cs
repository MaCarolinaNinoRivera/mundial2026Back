using FantasyWorldCup.Application.Teams.Interfaces;

namespace FantasyWorldCup.Application.Teams.UseCases.UpdateTeamKit;

public class UpdateTeamKitHandler
{
    private readonly ITeamRepository _teamRepo;

    public UpdateTeamKitHandler(ITeamRepository teamRepo)
    {
        _teamRepo = teamRepo;
    }

    public async Task ExecuteAsync(UpdateTeamKitCommand command)
    {
        var team = await _teamRepo.GetByUserIdAsync(command.UserId);

        if (team == null) throw new KeyNotFoundException("No tienes un equipo creado.");

        // Validar que sean colores hexadecimales válidos (opcional pero recomendado)
        team.ShirtPrimaryColor = command.PrimaryColor;
        team.ShirtSecondaryColor = command.SecondaryColor;

        await _teamRepo.SaveChangesAsync();
    }
}
