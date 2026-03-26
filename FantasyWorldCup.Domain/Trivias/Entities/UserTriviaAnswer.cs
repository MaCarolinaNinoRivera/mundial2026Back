namespace FantasyWorldCup.Domain.Trivias.Entities;

public class UserTriviaAnswer
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TriviaQuestionId { get; set; }
    public string? SelectedAnswer { get; set; } = string.Empty;
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
    public int PointsEarned { get; set; }
}
