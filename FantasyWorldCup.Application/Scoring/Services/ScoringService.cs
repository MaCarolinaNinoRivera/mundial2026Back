using FantasyWorldCup.Application.Scoring.Interfaces;
using FantasyWorldCup.Domain.Scoring.Entities;

namespace FantasyWorldCup.Application.Scoring.Services;

public class ScoringService
{
    private readonly IScoringRepository _repo;

    public ScoringService(IScoringRepository repo)
    {
        _repo = repo;
    }

    public async Task<int> CalculateEventPoints(string ruleKey, string? positionType)
    {
        var rules = await _repo.GetAllRulesAsync();
        var rule = rules.FirstOrDefault(r =>
            r.RuleKey == ruleKey && r.PositionType == positionType);

        if (rule == null)
            rule = rules.FirstOrDefault(r => r.RuleKey == ruleKey && r.PositionType == null);

        return rule?.PointsValue ?? 0;
    }
}
