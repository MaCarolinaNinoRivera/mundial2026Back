namespace FantasyWorldCup.Application.Predictions.UseCases.UpsertPrediction;

public record UpsertPredictionCommand(
    Guid MatchId,
    int PredictedHomeGoals,
    int PredictedAwayGoals
);
