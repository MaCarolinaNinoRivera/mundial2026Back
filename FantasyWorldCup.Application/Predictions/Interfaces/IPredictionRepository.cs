using FantasyWorldCup.Domain.Predictions.Entities;

namespace FantasyWorldCup.Application.Predictions.Interfaces;

public interface IPredictionRepository
{
    Task<MatchPrediction?> GetByUserAndMatchAsync(Guid userId, Guid matchId);
    Task<IEnumerable<MatchPrediction>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<MatchPrediction>> GetByMatchIdAsync(Guid matchId);
    Task<bool> ArePointsAlreadyCalculatedAsync(Guid matchId);
    Task AddAsync(MatchPrediction prediction);
    Task UpdateAsync(MatchPrediction prediction);
    Task SaveChangesAsync();
}
