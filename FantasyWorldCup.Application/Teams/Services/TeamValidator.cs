using FantasyWorldCup.Domain.Teams.Entities;
using FantasyWorldCup.Domain.Countries.Entities;

namespace FantasyWorldCup.Application.Teams.Services;

public class TeamValidator
{
    public (bool Success, string Message) CanAddPlayer(UserTeam team, FootballPlayer newPlayer, List<FootballPlayer> currentPlayers)
    {
        // 1. Validar Presupuesto
        if (team.AvailableBudget < newPlayer.BasePrice)
            return (false, "Presupuesto insuficiente.");

        // 2. Validar Límite Total (15)
        if (currentPlayers.Count >= 15)
            return (false, "El equipo ya tiene el máximo de 15 jugadores.");

        // 3. Validar Límite por País (Max 5)
        var playersFromSameCountry = currentPlayers.Count(p => p.CountryId == newPlayer.CountryId);
        if (playersFromSameCountry >= 5)
            return (false, "No puedes tener más de 5 jugadores del mismo país.");

        // 4. Validar Límite por Posición (2-5-5-3)
        var positionCounts = currentPlayers.GroupBy(p => p.Position)
                                           .ToDictionary(g => g.Key, g => g.Count());

        int currentInPos = positionCounts.ContainsKey(newPlayer.Position) ? positionCounts[newPlayer.Position] : 0;

        bool isAllowed = newPlayer.Position switch
        {
            "Goalkeeper" => currentInPos < 2,
            "Defender" => currentInPos < 5,
            "Midfielder" => currentInPos < 5,
            "Forward" => currentInPos < 3,
            _ => false
        };

        if (!isAllowed)
            return (false, $"Ya tienes el cupo lleno para la posición de {newPlayer.Position}.");

        return (true, "OK");
    }
}
