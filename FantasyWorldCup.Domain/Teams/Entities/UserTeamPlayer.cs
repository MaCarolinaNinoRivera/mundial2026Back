using FantasyWorldCup.Domain.Countries.Entities;
using System.Text.Json.Serialization;

namespace FantasyWorldCup.Domain.Teams.Entities;

public class UserTeamPlayer
{
    public Guid Id { get; set; }
    public Guid UserTeamId { get; set; }
    public Guid FootballPlayerId { get; set; }
    public bool IsStarter { get; set; }
    public bool IsCaptain { get; set; }
    public string? PositionSlot { get; set; }
    public DateTime? LockedAt { get; set; }

    // Propiedades de Navegación
    public FootballPlayer Player { get; set; } = default!;

    [JsonIgnore]
    public UserTeam UserTeam { get; set; } = default!; // <--- AGREGA ESTA LÍNEA
}
