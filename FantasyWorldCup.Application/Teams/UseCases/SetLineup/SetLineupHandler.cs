using FantasyWorldCup.Application.Teams.Interfaces;
using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Domain.Teams.Entities;

namespace FantasyWorldCup.Application.Teams.UseCases.SetLineup;

public class SetLineupHandler
{
    private readonly ITeamRepository _teamRepo;
    private readonly IMatchRepository _matchRepo;

    public SetLineupHandler(ITeamRepository teamRepo, IMatchRepository matchRepo)
    {
        _teamRepo = teamRepo;
        _matchRepo = matchRepo;
    }

    public async Task Handle(Guid userId, SetLineupCommand command)
    {
        // 1. Validar que el partido existe y NO ha comenzado
        var match = await _matchRepo.GetByIdAsync(command.MatchId);
        if (match == null) throw new KeyNotFoundException("Partido no encontrado.");

        if (DateTime.UtcNow >= match.StartTime)
            throw new InvalidOperationException("No puedes modificar la alineación una vez iniciado el partido.");

        // 2. Validar que son exactamente 11 jugadores (o tu regla de negocio)
        if (command.PlayerIds.Count != 11)
            throw new ArgumentException("Debes seleccionar exactamente 11 jugadores titulares.");

        // 3. Obtener el equipo del usuario
        var userTeam = await _teamRepo.GetByUserIdAsync(userId);
        if (userTeam == null) throw new InvalidOperationException("El usuario no tiene un equipo creado.");

        // 4. Validar que todos los jugadores seleccionados pertenecen al equipo del usuario
        var myPlayers = await _teamRepo.GetTeamPlayersAsync(userTeam.Id);
        var myPlayerIds = myPlayers.Select(p => p.FootballPlayerId).ToList();

        if (command.PlayerIds.Any(id => !myPlayerIds.Contains(id)))
            throw new InvalidOperationException("Uno o más jugadores seleccionados no pertenecen a tu nómina.");

        // 5. Limpiar alineación anterior para este partido y guardar la nueva
        // Nota: Es mejor hacerlo en una transacción o mediante un método de repositorio dedicado
        await _teamRepo.ClearAndSetLineupAsync(userTeam.Id, command.MatchId, command.PlayerIds);
    }
}