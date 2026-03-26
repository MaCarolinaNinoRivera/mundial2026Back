namespace FantasyWorldCup.Application.Badges.Queries.GetBadges;

public class BadgeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public string? IconUrl { get; set; } = null!;
    public string CriteriaType { get; set; } = null!;
}

