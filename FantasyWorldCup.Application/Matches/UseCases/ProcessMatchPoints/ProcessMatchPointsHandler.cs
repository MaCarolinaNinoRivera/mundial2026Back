using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Application.Scoring.Interfaces;
using FantasyWorldCup.Application.Scoring.Services;
using FantasyWorldCup.Domain.Scoring.Entities; // Para el Ledger

namespace FantasyWorldCup.Application.Matches.UseCases.ProcessMatchPoints;

public class ProcessMatchPointsHandler
{
    private readonly IMatchRepository _matchRepo;
    private readonly IPointsRepository _pointsRepo;
    private readonly ScoringService _scoringService; // <-- Inyectamos el servicio

    public ProcessMatchPointsHandler(
        IMatchRepository matchRepo,
        IPointsRepository pointsRepo,
        ScoringService scoringService) // <-- Agregado al constructor
    {
        _matchRepo = matchRepo;
        _pointsRepo = pointsRepo;
        _scoringService = scoringService;
    }

    public async Task Handle(Guid matchId)
    {
        var stats = await _matchRepo.GetPlayerStatsByMatchIdAsync(matchId);

        foreach (var stat in stats)
        {
            int pointsForThisPlayer = 0;
            string position = stat.Player.Position;

            // Agregamos "await" a cada cálculo
            if (stat.Goals > 0)
                pointsForThisPlayer += stat.Goals * await _scoringService.CalculateEventPoints("GOAL_SCORED", position);

            if (stat.YellowCards > 0)
                pointsForThisPlayer += await _scoringService.CalculateEventPoints("YELLOW_CARD", null);

            if (stat.CleanSheet)
                pointsForThisPlayer += await _scoringService.CalculateEventPoints("CLEAN_SHEET", position);

            if (pointsForThisPlayer != 0)
            {
                var owners = await _pointsRepo.GetStartersByPlayerIdAsync(stat.PlayerId);

                foreach (var owner in owners)
                {
                    int finalPoints = owner.IsCaptain ? pointsForThisPlayer * 2 : pointsForThisPlayer;

                    _pointsRepo.AddPointLedger(new UserPointsLedger
                    {
                        Id = Guid.NewGuid(),
                        UserId = owner.UserTeam.UserId,
                        Points = finalPoints,
                        SourceId = stat.Id,
                        SourceType = "PLAYER_PERFORMANCE",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }
        await _pointsRepo.SaveChangesAsync();
    }
}
