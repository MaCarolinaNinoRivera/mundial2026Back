using FluentValidation;
using FantasyWorldCup.Application.Matches.UseCases.CreateMatch;

namespace FantasyWorldCup.Application.Matches.Validators; // <--- AGREGA ESTA LÍNEA

public class CreateMatchValidator : AbstractValidator<CreateMatchCommand>
{
    public CreateMatchValidator()
    {
        RuleFor(x => x.HomeCountryId)
            .NotEmpty().WithMessage("El país local es obligatorio.");

        RuleFor(x => x.AwayCountryId)
            .NotEmpty().WithMessage("El país visitante es obligatorio.")
            .NotEqual(x => x.HomeCountryId)
            .WithMessage("Un país no puede jugar contra sí mismo.");

        RuleFor(x => x.StartTime)
            .Must(BeInTheFuture)
            .WithMessage("La fecha del partido debe ser futura (mínimo 30 minutos desde ahora).");

        RuleFor(x => x.StartTime)
            .LessThan(new DateTime(2026, 08, 01, 0, 0, 0, DateTimeKind.Utc))
            .WithMessage("La fecha no puede ser posterior al fin del mundial.");
    }

    private bool BeInTheFuture(DateTime startTime)
    {
        return startTime > DateTime.UtcNow.AddMinutes(30);
    }
}
