namespace FantasyWorldCup.Domain.Scoring.Entities;

public class ScoringRule
{
    public Guid Id { get; set; }
    public string RuleKey { get; set; } = string.Empty;
    public string? PositionType { get; set; }
    public int PointsValue { get; set; }
    public string? Description { get; set; }
}
