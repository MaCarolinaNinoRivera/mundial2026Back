using FantasyWorldCup.Application.Predictions.Interfaces;
using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Domain.Scoring.Entities;

namespace FantasyWorldCup.Application.Predictions.UseCases.CalculatePoints;

public class CalculatePointsHandler
{
    private readonly IPredictionRepository _predictionRepo;
    private readonly IMatchRepository _matchRepo;
    private readonly IPointsRepository _pointsRepo;

    public CalculatePointsHandler(IPredictionRepository predictionRepo, IMatchRepository matchRepo, IPointsRepository pointsRepo)
    {
        _predictionRepo = predictionRepo;
        _matchRepo = matchRepo;
        _pointsRepo = pointsRepo;
    }

    public async Task Handle(Guid matchId)
    {
        var match = await _matchRepo.GetByIdAsync(matchId);
        if (match == null || match.HomeGoals == null || match.AwayGoals == null)
            throw new InvalidOperationException("The match does not yet have an official result.");

        var alreadyProcessed = await _predictionRepo.ArePointsAlreadyCalculatedAsync(matchId);

        if (alreadyProcessed)
        {
            throw new InvalidOperationException("Los puntos de las predicciones para este partido ya han sido calculados.");
        }

        var predictions = await _predictionRepo.GetByMatchIdAsync(matchId);

        foreach (var pred in predictions)
        {
            int points = 0;

            bool exactScore = pred.PredictedHomeGoals == match.HomeGoals &&
                             pred.PredictedAwayGoals == match.AwayGoals;

            bool correctWinner = Math.Sign(pred.PredictedHomeGoals - pred.PredictedAwayGoals) ==
                                Math.Sign(match.HomeGoals.Value - match.AwayGoals.Value);

            if (exactScore)
            {
                points = 3;
            }
            else if (correctWinner)
            {
                points = 1;
            }

            pred.PointsEarned = points;

            if (points > 0)
            {
                _pointsRepo.AddPointLedger(new UserPointsLedger
                {
                    Id = Guid.NewGuid(),
                    UserId = pred.UserId,
                    Points = points,
                    SourceId = pred.Id, // ID de la predicción
                    SourceType = "PREDICTION",
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _predictionRepo.UpdateAsync(pred);
        }

        await _predictionRepo.SaveChangesAsync();
    }
}
