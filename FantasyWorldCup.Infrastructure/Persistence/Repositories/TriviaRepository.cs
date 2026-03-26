using Microsoft.EntityFrameworkCore;
using FantasyWorldCup.Application.Trivias.Interfaces;
using FantasyWorldCup.Domain.Trivias.Entities;

namespace FantasyWorldCup.Infrastructure.Persistence.Repositories;

public class TriviaRepository : ITriviaRepository
{
    private readonly AppDbContext _context;

    public TriviaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TriviaQuestion>> GetAllAsync()
    {
        return await _context.TriviaQuestions.ToListAsync();
    }

    public async Task<TriviaQuestion?> GetByIdAsync(Guid id)
    {
        return await _context.TriviaQuestions.FindAsync(id);
    }

    public async Task<TriviaQuestion?> GetRandomQuestionAsync()
    {
        return await _context.TriviaQuestions
            .OrderBy(r => Guid.NewGuid())
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(TriviaQuestion question)
    {
        await _context.TriviaQuestions.AddAsync(question);
    }

    public void Update(TriviaQuestion question)
    {
        _context.TriviaQuestions.Update(question);
    }

    public void Remove(TriviaQuestion question)
    {
        _context.TriviaQuestions.Remove(question);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Guid>> GetUserAnsweredQuestionIdsAsync(Guid userId)
    {
        return await _context.Set<UserTriviaAnswer>()
            .Where(a => a.UserId == userId)
            .Select(a => a.TriviaQuestionId)
            .ToListAsync();
    }

    public async Task<TriviaQuestion?> GetAvailableRandomQuestionAsync(IEnumerable<Guid> excludedIds)
    {
        return await _context.TriviaQuestions
            .Where(q => !excludedIds.Contains(q.Id))
            .OrderBy(r => Guid.NewGuid())
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasUserAnsweredAsync(Guid userId, Guid questionId)
    {
        return await _context.Set<UserTriviaAnswer>()
            .AnyAsync(a => a.UserId == userId && a.TriviaQuestionId == questionId);
    }

    public async Task AddAnswerAsync(UserTriviaAnswer answer)
    {
        await _context.Set<UserTriviaAnswer>().AddAsync(answer);
    }

    public async Task<bool> HasUserAnsweredTodayAsync(Guid userId)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Set<UserTriviaAnswer>()
            .AnyAsync(a => a.UserId == userId && a.AnsweredAt.Date == today);
    }

    public async Task<int> GetCurrentStreakAsync(Guid userId)
    {
        var answers = await _context.UserTriviaAnswers
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AnsweredAt)
            .ToListAsync();

        int streak = 0;
        foreach (var answer in answers)
        {
            if (answer.PointsEarned > 0) streak++;
            else break; // La racha se corta al primer error
        }
        return streak;
    }

    public async Task CreateEmptyAnswerAsync(Guid userId, Guid questionId)
    {
        var attempt = new UserTriviaAnswer
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TriviaQuestionId = questionId,
            SelectedAnswer = null, // Marcamos que solo se mostró
            AnsweredAt = DateTime.UtcNow,
            PointsEarned = 0
        };
        _context.UserTriviaAnswers.Add(attempt);
        await _context.SaveChangesAsync();
    }

    public async Task<UserTriviaAnswer?> GetAttemptAsync(Guid userId, Guid questionId)
    {
        // Buscamos la respuesta de hoy que aún no tenga opción seleccionada (intento vacío)
        return await _context.UserTriviaAnswers
            .FirstOrDefaultAsync(a => a.UserId == userId &&
                                      a.TriviaQuestionId == questionId &&
                                      string.IsNullOrEmpty(a.SelectedAnswer));
    }

    public async Task UpdateAnswerAsync(UserTriviaAnswer answer)
    {
        // Le avisamos a Entity Framework que este objeto cambió
        _context.UserTriviaAnswers.Update(answer);
        await Task.CompletedTask; // Solo para cumplir con la firma async si no haces save aquí
    }
}
