using FantasyWorldCup.Application.Trivias.Interfaces;
using FantasyWorldCup.Domain.Trivias.Entities;

namespace FantasyWorldCup.Application.Trivias.UseCases.GetNextQuestion;

public class GetNextQuestionHandler
{
    private readonly ITriviaRepository _repo;

    public GetNextQuestionHandler(ITriviaRepository repo) => _repo = repo;

    public async Task<TriviaQuestion?> Handle(Guid userId)
    {
        var alreadyPlayedToday = await _repo.HasUserAnsweredTodayAsync(userId);

        if (alreadyPlayedToday)
        {
            throw new InvalidOperationException("Ya respondiste la trivia de hoy. ¡Vuelve mañana por más preguntas!");
        }

        var answeredIds = await _repo.GetUserAnsweredQuestionIdsAsync(userId);
        var question = await _repo.GetAvailableRandomQuestionAsync(answeredIds);

        if (question != null)
        {
            await _repo.CreateEmptyAnswerAsync(userId, question.Id);
        }

        return question;
    }
}
