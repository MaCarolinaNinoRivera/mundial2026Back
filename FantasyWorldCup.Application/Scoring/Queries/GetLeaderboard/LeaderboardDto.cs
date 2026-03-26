namespace FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard;

public class LeaderboardDto
{
    public Guid UserId { get; set; }
    public int Position { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public decimal TotalPoints { get; set; }
    public int RankChange { get; set; }
}
