namespace FantasyWorldCup.Domain.Predictions.Entities;

public class MatchPrediction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MatchId { get; set; }
    public int PredictedHomeGoals { get; set; }
    public int PredictedAwayGoals { get; set; }
    public DateTime? LockedAt { get; set; }
    public int PointsEarned { get; set; }
}
