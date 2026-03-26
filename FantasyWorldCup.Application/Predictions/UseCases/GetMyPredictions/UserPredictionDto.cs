namespace FantasyWorldCup.Application.Predictions.UseCases.GetMyPredictions;

public record UserPredictionDto(
    Guid PredictionId,
    Guid MatchId,
    string HomeTeamName,
    string AwayTeamName,
    int PredictedHomeGoals,
    int PredictedAwayGoals,
    DateTime MatchStartTime,
    int PointsEarned
);
