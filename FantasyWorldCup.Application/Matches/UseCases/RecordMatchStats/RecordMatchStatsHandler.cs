using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Domain.Matches.Entities;

namespace FantasyWorldCup.Application.Matches.UseCases.RecordMatchStats;

public class RecordMatchStatsHandler
{
    private readonly IMatchRepository _matchRepo;
    private readonly IPointsRepository _pointsRepo;

    public RecordMatchStatsHandler(IMatchRepository matchRepo, IPointsRepository pointsRepo)
    {
        _matchRepo = matchRepo;
        _pointsRepo = pointsRepo;
    }

    public async Task Handle(RecordMatchStatsCommand command)
    {
        var match = await _matchRepo.GetByIdAsync(command.MatchId);
        if (match == null) throw new Exception("Partido no encontrado.");

        foreach (var entry in command.Stats)
        {
            var stat = new PlayerMatchStat
            {
                Id = Guid.NewGuid(),
                MatchId = command.MatchId,
                PlayerId = entry.FootballPlayerId,
                MinutesPlayed = entry.MinutesPlayed,
                Goals = entry.Goals,
                Assists = entry.Assists,
                YellowCards = entry.YellowCards,
                RedCards = entry.RedCards,
                PenaltySaved = entry.PenaltySaved,
                CleanSheet = entry.CleanSheet,
                CreatedAt = DateTime.UtcNow
            };

            // Usamos el repositorio de puntos para agregar la estadística
            _pointsRepo.AddPlayerMatchStat(stat);
        }

        await _pointsRepo.SaveChangesAsync();
    }
}
