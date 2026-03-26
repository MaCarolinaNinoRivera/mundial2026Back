using FantasyWorldCup.Domain.Common;
namespace FantasyWorldCup.Domain.Teams.Entities;

public class UserMatchSubstitution : BaseEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid UserTeamId { get; set; }
    public Guid MatchId { get; set; }
    public Guid LineupPlayerOutId { get; set; }
    public Guid LineupPlayerInId { get; set; }
    public DateTime SubstitutionTime { get; set; } // Tiempo real (reloj)
    public int SubstitutionMinute { get; set; }    // <--- AÑADIR ESTA LÍNEA (Minuto del partido)
    public DateTime CreatedAt { get; set; }

    // Propiedades de navegación para evitar errores de compilación en el Handler
    public virtual UserMatchLineup LineupPlayerOut { get; set; } = null!;
    public virtual UserMatchLineup LineupPlayerIn { get; set; } = null!;
}
