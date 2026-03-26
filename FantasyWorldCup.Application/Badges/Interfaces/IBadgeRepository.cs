using FantasyWorldCup.Domain.Badges.Entities;
using FantasyWorldCup.Domain.Scoring.Entities;
using FantasyWorldCup.Application.Badges.Queries.GetBadges;

namespace FantasyWorldCup.Application.Badges.Interfaces;

public interface IBadgeRepository
{
    Task<Badge?> GetBadgeByCriteriaAsync(string criteriaType);
    Task<bool> UserHasBadgeAsync(Guid userId, Guid badgeId);
    Task AwardBadgeAsync(UserBadge userBadge, UserPointsLedger? pointEntry);
    Task<List<BadgeDto>> GetAllBadgesAsync();
}
