using FantasyWorldCup.Application.Trivias.Interfaces;
using FantasyWorldCup.Domain.Trivias.Entities;

namespace FantasyWorldCup.Application.Trivias.UseCases.CreateQuestion;

public record CreateQuestionCommand(
    string QuestionText, string OptionA, string OptionB,
    string OptionC, string CorrectOption,
    int PointsValue);

public class CreateQuestionHandler
{
    private readonly ITriviaRepository _repo;
    public CreateQuestionHandler(ITriviaRepository repo) => _repo = repo;

    public async Task<Guid> Handle(CreateQuestionCommand command)
    {
        var question = new TriviaQuestion
        {
            Id = Guid.NewGuid(),
            QuestionText = command.QuestionText,
            OptionA = command.OptionA,
            OptionB = command.OptionB,
            OptionC = command.OptionC,
            CorrectOption = command.CorrectOption.ToUpper(),
            PointsValue = command.PointsValue
        };

        await _repo.AddAsync(question);
        await _repo.SaveChangesAsync();
        return question.Id;
    }
}
