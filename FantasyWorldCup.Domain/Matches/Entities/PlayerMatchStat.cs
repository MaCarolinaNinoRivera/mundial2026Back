using FantasyWorldCup.Domain.Countries.Entities;

namespace FantasyWorldCup.Domain.Matches.Entities;

public class PlayerMatchStat
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public Guid PlayerId { get; set; } // <--- Cambiado de FootballPlayerId a PlayerId
    public int MinutesPlayed { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public bool CleanSheet { get; set; }
    public int PenaltySaved { get; set; } // <--- Asegúrate de que exista
    public DateTime CreatedAt { get; set; } // <--- Asegúrate de que exista

    // Navegación
    public FootballPlayer Player { get; set; } = default!;
}
