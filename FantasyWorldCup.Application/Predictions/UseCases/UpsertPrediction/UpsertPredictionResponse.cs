namespace FantasyWorldCup.Application.Predictions.UseCases.UpsertPrediction;

public class UpsertPredictionResponse
{
    public string Message { get; set; } = "Predicción registrada con éxito.";
    public bool EarnedBadge { get; set; }
    public string? BadgeName { get; set; }
}
