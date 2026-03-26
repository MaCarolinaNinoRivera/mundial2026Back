using FantasyWorldCup.Application.Teams.Interfaces;
using FantasyWorldCup.Application.Matches.Interfaces;

namespace FantasyWorldCup.Application.Teams.UseCases.MakeSubstitution;

public class MakeSubstitutionHandler
{
    private readonly ITeamRepository _teamRepo;
    private readonly IMatchRepository _matchRepo;

    public MakeSubstitutionHandler(ITeamRepository teamRepo, IMatchRepository matchRepo)
    {
        _teamRepo = teamRepo;
        _matchRepo = matchRepo;
    }

    public async Task Handle(Guid userId, Guid matchId, Guid lineupIdOut, Guid teamPlayerIdIn)
    {
        // 1. Validar si el match existe
        var match = await _matchRepo.GetByIdAsync(matchId);
        if (match == null)
            throw new KeyNotFoundException($"El partido con ID {matchId} no existe.");

        var userTeam = await _teamRepo.GetByUserIdAsync(userId);
        if (userTeam == null)
            throw new InvalidOperationException("No tienes un equipo creado.");

        var now = DateTime.UtcNow;

        // --- LÓGICA DE PRE-PARTIDO (Swap gratis) ---
        if (now < match.StartTime)
        {
            // Aquí enviamos el ID de la línea de titular y el ID del jugador que está en la banca
            await _teamRepo.SwapLineupPlayersBeforeMatchAsync(userId, matchId, lineupIdOut, teamPlayerIdIn);
            return;
        }

        // --- LÓGICA DE PARTIDO EN CURSO (Sustitución con costo) ---
        int currentSubs = await _teamRepo.GetSubstitutionCountAsync(userId, matchId);
        if (currentSubs >= 3)
            throw new InvalidOperationException("Ya agotaste tus 3 cambios oficiales.");

        await _teamRepo.ExecuteSubstitutionAsync(userId, matchId, lineupIdOut, teamPlayerIdIn);
    }
}