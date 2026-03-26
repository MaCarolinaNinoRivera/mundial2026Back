namespace FantasyWorldCup.Application.Trivias.UseCases.AnswerQuestion;

public class AnswerQuestionResponse
{
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool EarnedBadge { get; set; }
    public string? BadgeName { get; set; }
}
