using FantasyWorldCup.Application.Trivias.Interfaces;
using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Domain.Trivias.Entities;
using FantasyWorldCup.Domain.Scoring.Entities;
using FantasyWorldCup.Application.Badges.Services;

namespace FantasyWorldCup.Application.Trivias.UseCases.AnswerQuestion;

public record AnswerQuestionCommand(Guid QuestionId, string SelectedOption);

public class AnswerQuestionHandler
{
    private readonly ITriviaRepository _repo;
    private readonly IPointsRepository _pointsRepo;
    private readonly BadgeEngineService _badgeEngine;

    public AnswerQuestionHandler(
        ITriviaRepository repo,
        IPointsRepository pointsRepo,
        BadgeEngineService badgeEngine) // NUEVO: Agregado al constructor
    {
        _repo = repo;
        _pointsRepo = pointsRepo;
        _badgeEngine = badgeEngine;
    }

    public async Task<AnswerQuestionResponse> Handle(Guid userId, AnswerQuestionCommand command)
    {
        // 1. Buscamos el intento que se creó cuando el usuario vio la pregunta (GetNext)
        // Buscamos un registro de hoy para este usuario y esta pregunta que no tenga respuesta aún
        var existingAttempt = await _repo.GetAttemptAsync(userId, command.QuestionId);

        if (existingAttempt == null)
            throw new InvalidOperationException("No se encontró un intento activo para esta trivia. Debes solicitar la pregunta primero.");

        if (!string.IsNullOrEmpty(existingAttempt.SelectedAnswer))
            throw new InvalidOperationException("Ya respondiste esta pregunta.");

        var question = await _repo.GetByIdAsync(command.QuestionId);
        if (question == null) throw new KeyNotFoundException("Question not found.");

        // 2. Lógica de validación de respuesta
        bool isCorrect = question.CorrectOption.Trim().ToUpper() == command.SelectedOption.Trim().ToUpper();
        int points = isCorrect ? question.PointsValue : 0;

        // 3. ACTUALIZAMOS el registro existente en lugar de crear uno nuevo
        existingAttempt.SelectedAnswer = command.SelectedOption;
        existingAttempt.PointsEarned = points;
        existingAttempt.AnsweredAt = DateTime.UtcNow;

        // Guardamos los cambios en la respuesta de trivia
        await _repo.UpdateAnswerAsync(existingAttempt);

        // 4. Lógica de puntos (se mantiene igual)
        if (isCorrect && points > 0)
        {
            _pointsRepo.AddPointLedger(new UserPointsLedger
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Points = points,
                SourceId = existingAttempt.Id,
                SourceType = "TRIVIA",
                CreatedAt = DateTime.UtcNow
            });
        }

        await _repo.SaveChangesAsync();

        // 5. Lógica de insignias (se mantiene igual)
        bool wonBadge = false;
        if (isCorrect)
        {
            int currentStreak = await _repo.GetCurrentStreakAsync(userId);
            wonBadge = await _badgeEngine.CheckAndAwardTriviaExpertAsync(userId, currentStreak);
        }

        return new AnswerQuestionResponse
        {
            IsCorrect = isCorrect,
            PointsEarned = points,
            Message = isCorrect
                ? (wonBadge ? "¡INCREÍBLE! 10 aciertos seguidos. Eres un experto." : "¡Correcto!")
                : "Respuesta incorrecta. ¡Sigue intentando!",
            EarnedBadge = wonBadge,
            BadgeName = wonBadge ? "El Experto en Trivias" : null
        };
    }
}
