using FantasyWorldCup.Application.Scoring.Interfaces;
using FantasyWorldCup.Domain.Scoring.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyWorldCup.Application.Scoring.UseCases.RecordRankSnapshot;

public class RecordRankSnapshotHandler
{
    private readonly IScoringRepository _scoringRepository;

    public RecordRankSnapshotHandler(IScoringRepository scoringRepository)
    {
        _scoringRepository = scoringRepository;
    }

    public async Task ExecuteAsync()
    {
        // 1. Obtener el leaderboard actual ordenado por puntos
        var currentLeaderboard = await _scoringRepository.GetLeaderboardAsync();

        // 2. Mapear a la entidad histórica (user_rank_history)
        // Asumimos que el leaderboard ya viene ordenado
        var historyRecords = currentLeaderboard.Select((user, index) => new UserRankHistory
        {
            Id = Guid.NewGuid(),
            UserId = user.UserId,
            RankPosition = index + 1, // La posición es el índice + 1
            TotalPoints = user.TotalPoints,
            CalculatedAt = DateTime.UtcNow // Siempre UTC en DB
        }).ToList();

        // 3. Guardar masivamente en la base de datos
        await _scoringRepository.SaveRankHistoryAsync(historyRecords);
    }
}
