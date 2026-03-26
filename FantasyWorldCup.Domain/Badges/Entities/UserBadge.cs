using FantasyWorldCup.Domain.Auth.Entities;
namespace FantasyWorldCup.Domain.Badges.Entities;

public class UserBadge
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BadgeId { get; set; }
    public DateTime AwardedAt { get; set; }
    public virtual Badge Badge { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
