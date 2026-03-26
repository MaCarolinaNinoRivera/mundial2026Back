using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Domain.Matches.Entities;
using FantasyWorldCup.Domain.Teams.Entities;

namespace FantasyWorldCup.Application.Matches.UseCases.RecordMatchEvent;

public class RecordMatchEventHandler
{
    private readonly IPointsRepository _repository;

    public RecordMatchEventHandler(IPointsRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(RecordMatchEventCommand command)
    {
        // 1. Validaciones de existencia
        var match = await _repository.GetMatchByIdAsync(command.MatchId);
        if (match == null) throw new KeyNotFoundException("El partido no existe.");

        var playerExists = await _repository.PlayerExistsAsync(command.PlayerId);
        if (!playerExists) throw new KeyNotFoundException($"Jugador {command.PlayerId} no existe.");

        // 2. Crear el evento base (GOL, AMARILLA, etc.)
        var eventType = command.EventType.ToUpper().Trim();
        var matchEvent = new MatchEvent
        {
            Id = Guid.NewGuid(),
            MatchId = command.MatchId,
            PlayerId = command.PlayerId,
            EventType = eventType,
            Minute = command.Minute,
            RelatedPlayerId = command.RelatedPlayerId,
            CreatedAt = DateTime.UtcNow
        };
        _repository.AddMatchEvent(matchEvent);

        // 4. PERSISTENCIA
        await _repository.SaveChangesAsync();
    }
}
