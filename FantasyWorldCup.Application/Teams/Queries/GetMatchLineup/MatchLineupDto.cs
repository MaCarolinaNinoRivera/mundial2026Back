namespace FantasyWorldCup.Application.Teams.Queries.GetMatchLineup;

public class MatchLineupDto
{
    public Guid LineupId { get; set; }
    public Guid FootballPlayerId { get; set; }
    public string PlayerName { get; set; } = null!;
    public string Position { get; set; } = null!;
    public int ShirtNumber { get; set; }
    public bool IsStarter { get; set; }
    public bool IsSubstituted { get; set; }
    public string? SubstitutedByPlayerName { get; set; }
    public string TeamPrimaryColor { get; set; } = string.Empty;
    public string TeamSecondaryColor { get; set; } = string.Empty;
    public string CountryName { get; set; } = null!;
}
