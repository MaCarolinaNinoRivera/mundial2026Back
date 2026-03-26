using Microsoft.EntityFrameworkCore;
using FantasyWorldCup.Application.Predictions.Interfaces;
using FantasyWorldCup.Domain.Predictions.Entities;
using FantasyWorldCup.Domain.Scoring.Entities;
using FantasyWorldCup.Infrastructure.Persistence;

namespace FantasyWorldCup.Infrastructure.Persistence.Repositories;

public class PredictionRepository : IPredictionRepository
{
    private readonly AppDbContext _context;

    public PredictionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MatchPrediction?> GetByUserAndMatchAsync(Guid userId, Guid matchId)
    {
        return await _context.MatchPredictions
            .FirstOrDefaultAsync(p => p.UserId == userId && p.MatchId == matchId);
    }

    public async Task<IEnumerable<MatchPrediction>> GetByMatchIdAsync(Guid matchId)
    {
        return await _context.MatchPredictions
            .Where(p => p.MatchId == matchId)
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchPrediction>> GetByUserIdAsync(Guid userId)
    {
        return await _context.MatchPredictions
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(MatchPrediction prediction)
    {
        await _context.MatchPredictions.AddAsync(prediction);
    }

    public async Task<bool> ArePointsAlreadyCalculatedAsync(Guid matchId)
    {
        // 1. Obtenemos primero los IDs de las predicciones de ese partido
        var predictionIds = _context.MatchPredictions
            .Where(p => p.MatchId == matchId)
            .Select(p => p.Id);

        // 2. Comparamos en el Ledger si el SourceId está en esa lista
        return await _context.Set<UserPointsLedger>()
            .AnyAsync(l => l.SourceType == "PREDICTION" &&
                           predictionIds.Any(id => id == l.SourceId)); // Usamos Any en lugar de Contains
    }

    public async Task UpdateAsync(MatchPrediction prediction)
    {
        _context.MatchPredictions.Update(prediction);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
