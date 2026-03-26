namespace FantasyWorldCup.Application.Teams.Queries.GetMyTeam;

public class UserTeamDto
{
    public Guid Id { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal AvailableBudget { get; set; }
    public bool Locked { get; set; }
    public decimal TeamValue => TeamPlayers.Sum(p => p.PurchasePrice);
    public string ShirtPrimaryColor { get; set; } = string.Empty;
    public string ShirtSecondaryColor { get; set; } = string.Empty;
    public List<TeamPlayerDto> TeamPlayers { get; set; } = new();
}

public class TeamPlayerDto
{
    public Guid Id { get; set; }
    public Guid FootballPlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public int ShirtNumber { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public bool IsStarter { get; set; }
    public bool IsCaptain { get; set; }
}
