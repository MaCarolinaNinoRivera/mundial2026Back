using FantasyWorldCup.Domain.Trivias.Entities;

namespace FantasyWorldCup.Application.Trivias.Interfaces;

public interface ITriviaRepository
{
    Task<IEnumerable<TriviaQuestion>> GetAllAsync();
    Task<TriviaQuestion?> GetByIdAsync(Guid id);
    Task<TriviaQuestion?> GetRandomQuestionAsync();
    Task AddAsync(TriviaQuestion question);
    void Update(TriviaQuestion question);
    void Remove(TriviaQuestion question);
    Task SaveChangesAsync();
    Task<IEnumerable<Guid>> GetUserAnsweredQuestionIdsAsync(Guid userId);
    Task<TriviaQuestion?> GetAvailableRandomQuestionAsync(IEnumerable<Guid> excludedIds);
    Task AddAnswerAsync(UserTriviaAnswer answer);
    Task<bool> HasUserAnsweredAsync(Guid userId, Guid questionId);
    Task<bool> HasUserAnsweredTodayAsync(Guid userId);
    Task<int> GetCurrentStreakAsync(Guid userId);
    Task CreateEmptyAnswerAsync(Guid userId, Guid questionId);
    Task<UserTriviaAnswer?> GetAttemptAsync(Guid userId, Guid questionId);
    Task UpdateAnswerAsync(UserTriviaAnswer answer);
}
