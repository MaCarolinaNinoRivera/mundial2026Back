namespace FantasyWorldCup.Application.Trivias.UseCases.UpdateQuestion;

public record UpdateQuestionCommand(
    string QuestionText,
    string OptionA,
    string OptionB,
    string OptionC,
    string CorrectOption,
    int PointsValue
);