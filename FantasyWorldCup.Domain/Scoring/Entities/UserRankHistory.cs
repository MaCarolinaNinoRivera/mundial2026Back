namespace FantasyWorldCup.Domain.Scoring.Entities;

public class UserRankHistory
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int RankPosition { get; set; }
    public decimal TotalPoints { get; set; }
    public DateTime CalculatedAt { get; set; }
}
