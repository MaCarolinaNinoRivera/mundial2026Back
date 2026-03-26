namespace FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard;

public class UserBadgesCountDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int TotalBadges { get; set; }
    // Agrupamos por tipo para que el front sepa qué copas tiene
    public List<BadgeDetailDto> Badges { get; set; } = new();
}

public class BadgeDetailDto
{
    public string Name { get; set; } = string.Empty;
    public string CriteriaType { get; set; } = string.Empty;
}
