using FantasyWorldCup.Application.Teams.Interfaces;

namespace FantasyWorldCup.Application.Teams.Queries.GetMatchLineup;

public class GetMatchLineupHandler
{
    private readonly ITeamRepository _teamRepo;

    public GetMatchLineupHandler(ITeamRepository teamRepo)
    {
        _teamRepo = teamRepo;
    }

    public async Task<List<MatchLineupDto>> Handle(Guid userId, Guid matchId)
    {
        var team = await _teamRepo.GetByUserIdAsync(userId);
        if (team == null) throw new KeyNotFoundException("Equipo no encontrado.");

        var lineup = await _teamRepo.GetMatchLineupAsync(matchId, userId);

        // Definimos el orden táctico
        var positionOrder = new Dictionary<string, int>
        {
            { "Goalkeeper", 1 },
            { "Defender", 2 },
            { "Midfielder", 3 },
            { "Forward", 4 }
        };

        return lineup
            .OrderBy(l => positionOrder.GetValueOrDefault(l.UserTeamPlayer.Player.Position, 99))
            .Select(l => new MatchLineupDto
            {
                LineupId = l.Id,
                FootballPlayerId = l.UserTeamPlayer.FootballPlayerId,
                PlayerName = l.UserTeamPlayer.Player.Name,
                ShirtNumber = l.UserTeamPlayer.Player.ShirtNumber,
                Position = l.UserTeamPlayer.Player.Position,
                IsStarter = l.IsStarter,
                IsSubstituted = l.IsSubstituted,
                // Si fue sustituido, mostramos el nombre del que entró
                SubstitutedByPlayerName = l.SubstitutedBy?.UserTeamPlayer.Player.Name,
                CountryName = l.UserTeamPlayer.Player.Country?.Name ?? "N/A",
            })
            .ToList();
    }
}
