using FantasyWorldCup.Application.Matches.Interfaces; // Para IPointsRepository
using FantasyWorldCup.Application.Auth.Interfaces;    // Para IUserRepository
using FantasyWorldCup.Application.Scoring.Interfaces;

namespace FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard;

public class GetLeaderboardHandler
{
    private readonly IPointsRepository _pointsRepo;
    private readonly IUserRepository _userRepo;
    private readonly IScoringRepository _repository;

    public GetLeaderboardHandler(IPointsRepository pointsRepo, IUserRepository userRepo, IScoringRepository repository)
    {
        _pointsRepo = pointsRepo;
        _userRepo = userRepo;
        _repository = repository;
    }

    public async Task<List<LeaderboardDto>> Handle()
    {
        // El repositorio debe encargarse de la suma pesada en la DB
        return await _pointsRepo.GetGlobalLeaderboardAsync();
    }
}
