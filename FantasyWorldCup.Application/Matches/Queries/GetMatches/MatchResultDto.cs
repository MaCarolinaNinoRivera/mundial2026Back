namespace FantasyWorldCup.Application.Matches.Queries.GetMatches;

public class MatchResultDto
{
    public Guid IdMatch { get; set; }
    public string GroupA { get; set; } = string.Empty;
    public string Local { get; set; } = string.Empty;
    public string GroupB { get; set; } = string.Empty;
    public string Visitante { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public int? HomeGoals { get; set; }
    public int? AwayGoals { get; set; }
    public string Status { get; set; } = string.Empty;
}
