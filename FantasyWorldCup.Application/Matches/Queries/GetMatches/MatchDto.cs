namespace FantasyWorldCup.Application.Matches.Queries.GetMatches;

public record MatchDto(
    Guid Id,
    string HomeCountryName,
    string AwayCountryName,
    DateTime StartTime,
    int? HomeGoals,
    int? AwayGoals,
    string Status
);
