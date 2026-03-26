namespace FantasyWorldCup.Application.Matches.UseCases.RecordMatchEvent;

public record RecordMatchEventCommand(
    Guid MatchId,
    Guid PlayerId,
    string EventType, // "GOAL", "ASSIST", "YELLOW_CARD", etc.
    int Minute,
    Guid? RelatedPlayerId
);
