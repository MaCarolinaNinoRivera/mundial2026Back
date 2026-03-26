namespace FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard;

public class RankMovementDto
{
    public Guid UserId { get; set; }
    public string Alias { get; set; } = string.Empty; // <-- Aquí estaba la 'a' extra, ya la quité
    public int CurrentPosition { get; set; }
    public int PreviousPosition { get; set; }
    public int PositionChange => PreviousPosition - CurrentPosition;
    public decimal TotalPoints { get; set; } // Cambiado a decimal para evitar errores
    public decimal PointsDifference { get; set; } // Cambiado a decimal
}
