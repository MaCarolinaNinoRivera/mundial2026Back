using FantasyWorldCup.Application.Badges.Interfaces;

namespace FantasyWorldCup.Application.Badges.Queries.GetBadges;

public class GetBadgesHandler
{
    private readonly IBadgeRepository _repository;

    public GetBadgesHandler(IBadgeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BadgeDto>> Handle()
    {
        return await _repository.GetAllBadgesAsync();
    }
}
