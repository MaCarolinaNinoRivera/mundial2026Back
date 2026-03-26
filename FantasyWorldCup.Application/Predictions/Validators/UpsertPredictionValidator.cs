using FluentValidation;

namespace FantasyWorldCup.Application.Predictions.UseCases.UpsertPrediction;

public class UpsertPredictionValidator : AbstractValidator<UpsertPredictionCommand>
{
    public UpsertPredictionValidator()
    {
        RuleFor(x => x.MatchId).NotEmpty();

        RuleFor(x => x.PredictedHomeGoals)
            .GreaterThanOrEqualTo(0)
            .WithMessage("The home team's goals cannot be negative.");

        RuleFor(x => x.PredictedAwayGoals)
            .GreaterThanOrEqualTo(0)
            .WithMessage("The away team's goals cannot be negative.");
    }
}
