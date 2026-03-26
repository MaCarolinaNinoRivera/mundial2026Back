using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Application.Scoring.Services;
using FantasyWorldCup.Domain.Matches.Entities;
using FantasyWorldCup.Domain.Scoring.Entities;
using FantasyWorldCup.Domain.Teams.Entities;

namespace FantasyWorldCup.Application.Matches.UseCases.DistributePoints;

public class DistributePointsHandler
{
    private readonly IPointsRepository _repository;
    private readonly ScoringService _scoringService;

    public DistributePointsHandler(IPointsRepository repository, ScoringService scoringService)
    {
        _repository = repository;
        _scoringService = scoringService;
    }

    public async Task Handle(Guid matchId)
    {
        var match = await _repository.GetMatchByIdAsync(matchId);
        if (match == null) throw new KeyNotFoundException($"Match {matchId} no existe.");

        if (await _repository.AreMatchPointsDistributedAsync(matchId))
            throw new InvalidOperationException("Los puntos de este partido ya fueron procesados.");

        var matchEvents = await _repository.GetMatchEventsAsync(matchId);
        var allUserLineups = await _repository.GetAllLineupsForMatchAsync(matchId);

        foreach (var lineup in allUserLineups)
        {
            int totalPoints = 0;
            var player = lineup.UserTeamPlayer.Player;
            var substitution = await _repository.GetSubstitutionByLineupIdAsync(lineup.Id);
            var generalStat = await _repository.GetStatByPlayerAndMatchAsync(player.Id, matchId);

            // A. Puntos por Titularidad (Usa tu nueva regla STARTING_LINEUP)
            if (lineup.IsStarter)
            {
                totalPoints += await _scoringService.CalculateEventPoints("STARTING_LINEUP", player.Position);
            }

            // B. Puntos por Minutos Jugados (Dinámicos de tu tabla)
            if (generalStat != null)
            {
                if (generalStat.MinutesPlayed >= 1)
                    totalPoints += await _scoringService.CalculateEventPoints("MINUTES_PLAYED_1", player.Position);

                if (generalStat.MinutesPlayed >= 60)
                    totalPoints += await _scoringService.CalculateEventPoints("MINUTES_60", player.Position);
            }

            // C. Eventos Cronológicos (Goles, Tarjetas, etc.)
            var playerEventsInRealLife = matchEvents.Where(e => e.PlayerId == player.Id);

            foreach (var @event in playerEventsInRealLife)
            {
                bool pointsApply = false;

                if (lineup.IsStarter)
                {
                    if (substitution == null || @event.Minute <= substitution.SubstitutionMinute)
                        pointsApply = true;
                }
                else
                {
                    if (substitution != null && @event.Minute >= substitution.SubstitutionMinute)
                        pointsApply = true;
                }

                if (pointsApply)
                {
                    // Calcula usando el rule_key del evento (ej: GOAL_SCORED, YELLOW_CARD)
                    totalPoints += await _scoringService.CalculateEventPoints(@event.EventType, player.Position);
                }
            }

            // D. Asistencias (Desde la tabla de estadísticas generales)
            if (generalStat != null && generalStat.Assists > 0)
            {
                // Validamos si el jugador estaba "activo" para el usuario durante el partido
                // Para simplificar, si sumó minutos o eventos, le damos sus asistencias
                int pointsPerAssist = await _scoringService.CalculateEventPoints("ASSIST", player.Position);
                totalPoints += (pointsPerAssist * generalStat.Assists);
            }

            // E. Clean Sheet (Si tu regla lo permite para su posición)
            if (generalStat != null && generalStat.CleanSheet)
            {
                totalPoints += await _scoringService.CalculateEventPoints("CLEAN_SHEET", player.Position);
            }

            // F. Multiplicador de Capitán
            if (lineup.UserTeamPlayer.IsCaptain) totalPoints *= 2;

            // G. Registro Final
            if (totalPoints != 0)
            {
                _repository.AddPointLedger(new UserPointsLedger
                {
                    Id = Guid.NewGuid(),
                    UserId = lineup.UserTeam.UserId,
                    Points = totalPoints,
                    SourceId = matchId,
                    SourceType = "MATCH_PERFORMANCE",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await _repository.SaveChangesAsync();
    }
}
