namespace FantasyWorldCup.Domain.Trivias.Entities;

public class TriviaQuestion
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string CorrectOption { get; set; } = string.Empty;
    public int PointsValue { get; set; } = 5;
}
