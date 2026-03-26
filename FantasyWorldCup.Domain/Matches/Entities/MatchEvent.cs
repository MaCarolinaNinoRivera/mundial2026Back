using FantasyWorldCup.Domain.Common;
using FantasyWorldCup.Domain.Countries.Entities;

namespace FantasyWorldCup.Domain.Matches.Entities;

public class MatchEvent
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public Guid PlayerId { get; set; }
    public string EventType { get; set; } = null!; // GOAL, ASSIST, YELLOW_CARD, etc.
    public int Minute { get; set; }
    public Guid? RelatedPlayerId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Propiedades de navegación
    public virtual Match Match { get; set; } = null!;
    public virtual FootballPlayer Player { get; set; } = null!;
}
