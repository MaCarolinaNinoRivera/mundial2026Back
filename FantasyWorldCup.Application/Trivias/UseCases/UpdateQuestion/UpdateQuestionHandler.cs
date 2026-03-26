using FantasyWorldCup.Application.Trivias.Interfaces;
using FantasyWorldCup.Domain.Trivias.Entities;

namespace FantasyWorldCup.Application.Trivias.UseCases.UpdateQuestion;

public class UpdateQuestionHandler
{
    private readonly ITriviaRepository _repo;

    public UpdateQuestionHandler(ITriviaRepository repo) => _repo = repo;

    public async Task Handle(Guid id, UpdateQuestionCommand command)
    {
        var question = await _repo.GetByIdAsync(id);
        if (question == null) throw new KeyNotFoundException("Question not found");

        question.QuestionText = command.QuestionText;
        question.OptionA = command.OptionA;
        question.OptionB = command.OptionB;
        question.OptionC = command.OptionC;
        question.CorrectOption = command.CorrectOption.ToUpper();
        question.PointsValue = command.PointsValue;

        _repo.Update(question);
        await _repo.SaveChangesAsync();
    }
}
