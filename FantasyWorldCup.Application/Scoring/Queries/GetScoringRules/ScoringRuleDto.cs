namespace FantasyWorldCup.Application.Scoring.Queries.GetScoringRules;

public record ScoringRuleDto(
    string RuleKey,
    string? Position,
    int Points,
    string Description
);
