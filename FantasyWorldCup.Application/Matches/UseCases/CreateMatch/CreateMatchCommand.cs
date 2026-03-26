namespace FantasyWorldCup.Application.Matches.UseCases.CreateMatch;

public record CreateMatchCommand(
    Guid HomeCountryId,
    Guid AwayCountryId,
    DateTime StartTime
);
