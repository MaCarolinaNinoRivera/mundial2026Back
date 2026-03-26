namespace FantasyWorldCup.Application.Teams.Queries.GetAvailableBench;

public class BenchPlayerDto
{
    public Guid UserTeamPlayerId { get; set; } // ID para usar en el swap o substitution
    public Guid FootballPlayerId { get; set; }
    public string PlayerName { get; set; } = null!;
    public string Position { get; set; } = null!;
    public string CountryName { get; set; } = null!;
    public int ShirtNumber { get; set; }
}
