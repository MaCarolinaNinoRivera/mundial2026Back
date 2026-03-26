using FantasyWorldCup.Application.Predictions.Interfaces;
using FantasyWorldCup.Application.Matches.Interfaces;

namespace FantasyWorldCup.Application.Predictions.UseCases.GetMyPredictions;

public class GetMyPredictionsHandler
{
    private readonly IPredictionRepository _predictionRepo;
    private readonly IMatchRepository _matchRepo;

    public GetMyPredictionsHandler(IPredictionRepository predictionRepo, IMatchRepository matchRepo)
    {
        _predictionRepo = predictionRepo;
        _matchRepo = matchRepo;
    }

    public async Task<IEnumerable<UserPredictionDto>> Handle(Guid userId)
    {
        var predictions = await _predictionRepo.GetByUserIdAsync(userId);
        var matches = await _matchRepo.GetAllWithNamesAsync(); 

        var result = predictions.Select(p => {
            var match = matches.FirstOrDefault(m => m.Id == p.MatchId);
            return new UserPredictionDto(
                p.Id,
                p.MatchId,
                match?.HomeCountryName ?? "Unknown",
                match?.AwayCountryName ?? "Unknown",
                p.PredictedHomeGoals,
                p.PredictedAwayGoals,
                match?.StartTime ?? DateTime.MinValue,
                p.PointsEarned
            );
        });

        return result;
    }
}
