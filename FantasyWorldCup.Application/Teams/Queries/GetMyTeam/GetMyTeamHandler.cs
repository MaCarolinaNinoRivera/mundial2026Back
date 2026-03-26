using FantasyWorldCup.Application.Teams.Interfaces;

namespace FantasyWorldCup.Application.Teams.Queries.GetMyTeam;

public class GetMyTeamHandler
{
    private readonly ITeamRepository _teamRepo;

    public GetMyTeamHandler(ITeamRepository teamRepo)
    {
        _teamRepo = teamRepo;
    }

    public async Task<UserTeamDto> Handle(GetMyTeamQuery query)
    {
        // 1. Obtenemos el equipo a través de la interfaz del repositorio
        // Asegúrate de que tu repositorio ya incluya (Include) los jugadores y países internamente
        var team = await _teamRepo.GetFullTeamByUserIdAsync(query.UserId);

        if (team == null)
            throw new KeyNotFoundException("No tienes un equipo creado.");

        // 2. Diccionario de ordenamiento táctico
        var positionOrder = new Dictionary<string, int>
        {
            { "Goalkeeper", 1 },
            { "Defender", 2 },
            { "Midfielder", 3 },
            { "Forward", 4 }
        };

        // 3. Mapeo y ordenamiento
        return new UserTeamDto
        {
            Id = team.Id,
            TotalBudget = team.TotalBudget,
            AvailableBudget = team.AvailableBudget,
            Locked = team.Locked,
            ShirtPrimaryColor = team.ShirtPrimaryColor,
            ShirtSecondaryColor = team.ShirtSecondaryColor,
            TeamPlayers = team.TeamPlayers
                .OrderBy(tp => positionOrder.GetValueOrDefault(tp.Player.Position, 99))
                .ThenBy(tp => tp.Player.Name)
                .Select(tp => new TeamPlayerDto
                {
                    Id = tp.Id,
                    FootballPlayerId = tp.FootballPlayerId,
                    PlayerName = tp.Player.Name,
                    Position = tp.Player.Position,
                    CountryName = tp.Player.Country.Name,
                    ShirtNumber = tp.Player.ShirtNumber,
                    IsStarter = tp.IsStarter,
                    IsCaptain = tp.IsCaptain,
                    PurchasePrice = tp.Player.BasePrice
                }).ToList()
        };
    }
}
