using FantasyWorldCup.Application.Teams.Interfaces;

namespace FantasyWorldCup.Application.Teams.Queries.GetAvailableBench;

public class GetAvailableBenchHandler
{
    private readonly ITeamRepository _teamRepo;

    public GetAvailableBenchHandler(ITeamRepository teamRepo)
    {
        _teamRepo = teamRepo;
    }

    public async Task<List<BenchPlayerDto>> Handle(Guid userId, Guid matchId)
    {
        // 1. Obtener el equipo del usuario
        var team = await _teamRepo.GetByUserIdAsync(userId);
        if (team == null) throw new KeyNotFoundException("Equipo no encontrado.");

        // 2. Llamar al repositorio para obtener los jugadores que NO están en el lineup
        var benchPlayers = await _teamRepo.GetAvailableBenchAsync(team.Id, matchId);

        // 3. Mapear y ordenar por posición para que la banca se vea organizada
        var positionOrder = new Dictionary<string, int>
        {
            { "Goalkeeper", 1 },
            { "Defender", 2 },
            { "Midfielder", 3 },
            { "Forward", 4 }
        };

        return benchPlayers
            .OrderBy(tp => positionOrder.GetValueOrDefault(tp.Player.Position, 99))
            .Select(tp => new BenchPlayerDto
            {
                UserTeamPlayerId = tp.Id,
                FootballPlayerId = tp.FootballPlayerId,
                PlayerName = tp.Player.Name,
                Position = tp.Player.Position,
                ShirtNumber = tp.Player.ShirtNumber,
                CountryName = tp.Player.Country.Name
            })
            .ToList();
    }
}
