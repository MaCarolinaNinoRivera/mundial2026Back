namespace FantasyWorldCup.Domain.Scoring.Entities;

public class UserPointsLedger
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Points { get; set; }
    public Guid? SourceId { get; set; } // ID del Match o Trivia
    public string SourceType { get; set; } = string.Empty; // "MATCH", "TRIVIA", "PREDICTION"
    public DateTime CreatedAt { get; set; }
}
