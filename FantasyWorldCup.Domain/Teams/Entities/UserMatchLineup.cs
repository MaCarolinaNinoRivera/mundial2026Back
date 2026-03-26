namespace FantasyWorldCup.Domain.Teams.Entities;

public class UserMatchLineup
{
    public Guid Id { get; set; }
    public Guid UserTeamId { get; set; }
    public Guid MatchId { get; set; }
    public Guid UserTeamPlayerId { get; set; }
    public bool IsStarter { get; set; }
    public bool IsSubstituted { get; set; }
    public Guid? SubstitutedByLineupId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Propiedades de navegación
    public virtual UserTeam UserTeam { get; set; } = null!;
    public virtual UserTeamPlayer UserTeamPlayer { get; set; } = null!;
    public virtual UserMatchLineup? SubstitutedBy { get; set; }
}
