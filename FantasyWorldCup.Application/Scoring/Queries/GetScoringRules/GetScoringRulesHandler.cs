using FantasyWorldCup.Application.Scoring.Interfaces;
using FantasyWorldCup.Application.Scoring.Queries.GetScoringRules; // Donde está el ScoringRuleDto
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyWorldCup.Application.Scoring.Queries.GetScoringRules;

public class GetScoringRulesHandler
{
    private readonly IScoringRepository _repository;

    public GetScoringRulesHandler(IScoringRepository repository) => _repository = repository;

    public async Task<List<ScoringRuleDto>> Handle()
    {
        var rules = await _repository.GetActiveRulesAsync();
        return rules.Select(r => new ScoringRuleDto(
            r.RuleKey,
            r.PositionType,
            r.PointsValue,
            r.Description ?? string.Empty)).ToList();
    }
}
