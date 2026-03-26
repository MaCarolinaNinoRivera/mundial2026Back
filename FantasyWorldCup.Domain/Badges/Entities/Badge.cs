namespace FantasyWorldCup.Domain.Badges.Entities;

public class Badge
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public string CriteriaType { get; set; } = null!; // Ej: "EarlyBird", "TriviaExpert"
}
