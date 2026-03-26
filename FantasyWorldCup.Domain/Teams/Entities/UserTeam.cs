namespace FantasyWorldCup.Domain.Teams.Entities;

public class UserTeam
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal AvailableBudget { get; set; } // Dinero actual para comprar
    public bool Locked { get; set; }
    public DateTime CreatedAt { get; set; }

    public string ShirtPrimaryColor { get; set; } = "#0000FF";
    public string ShirtSecondaryColor { get; set; } = "#FFFFFF";

    // Relación con los jugadores del equipo
    public ICollection<UserTeamPlayer> TeamPlayers { get; set; } = new List<UserTeamPlayer>();
}
