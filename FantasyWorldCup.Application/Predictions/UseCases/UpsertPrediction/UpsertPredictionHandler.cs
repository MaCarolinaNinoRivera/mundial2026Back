using FantasyWorldCup.Application.Predictions.Interfaces;
using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Domain.Predictions.Entities;
using FantasyWorldCup.Application.Badges.Services;

namespace FantasyWorldCup.Application.Predictions.UseCases.UpsertPrediction;

public class UpsertPredictionHandler
{
    private readonly IPredictionRepository _predictionRepo;
    private readonly IMatchRepository _matchRepo;
    private readonly BadgeEngineService _badgeEngineService;

    public UpsertPredictionHandler(
        IPredictionRepository predictionRepo,
        IMatchRepository matchRepo,
        BadgeEngineService badgeEngineService)
    {
        _predictionRepo = predictionRepo;
        _matchRepo = matchRepo;
        _badgeEngineService = badgeEngineService;
    }

    // CAMBIO AQUÍ: Agregamos <UpsertPredictionResponse> al Task
    public async Task<UpsertPredictionResponse> Handle(Guid userId, UpsertPredictionCommand command)
    {
        var match = await _matchRepo.GetByIdAsync(command.MatchId);
        if (match is null)
            throw new KeyNotFoundException("El partido no existe.");

        var now = DateTime.UtcNow;
        var startTime = match.StartTime.ToUniversalTime();

        if (now >= startTime)
        {
            throw new InvalidOperationException(
                $"You cannot make or modify a prediction once the match has started. " +
                $"UtcNow: {now:yyyy-MM-dd HH:mm:ss} | Match StartTime: {startTime:yyyy-MM-dd HH:mm:ss}");
        }

        var existingPrediction = await _predictionRepo.GetByUserAndMatchAsync(userId, command.MatchId);

        if (existingPrediction is null)
        {
            var newPrediction = new MatchPrediction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MatchId = command.MatchId,
                PredictedHomeGoals = command.PredictedHomeGoals,
                PredictedAwayGoals = command.PredictedAwayGoals,
                LockedAt = startTime,
                PointsEarned = 0
            };
            await _predictionRepo.AddAsync(newPrediction);
        }
        else
        {
            existingPrediction.PredictedHomeGoals = command.PredictedHomeGoals;
            existingPrediction.PredictedAwayGoals = command.PredictedAwayGoals;
            existingPrediction.LockedAt = startTime;
            await _predictionRepo.UpdateAsync(existingPrediction);
        }

        await _predictionRepo.SaveChangesAsync();

        // Verificamos si ganó medalla
        var wonBadge = await _badgeEngineService.CheckAndAwardEarlyBirdAsync(userId, now, startTime);

        if (wonBadge)
        {
            return new UpsertPredictionResponse
            {
                Message = "¡Increíble! Has registrado tu apuesta con mucha antelación.",
                EarnedBadge = true,
                BadgeName = "El Madrugador"
            };
        }

        return new UpsertPredictionResponse { Message = "Predicción guardada. ¡Buena suerte!" };
    }
}
