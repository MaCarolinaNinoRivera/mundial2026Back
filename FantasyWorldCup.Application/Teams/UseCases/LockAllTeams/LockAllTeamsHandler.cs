using FantasyWorldCup.Application.Teams.Interfaces;

namespace FantasyWorldCup.Application.Teams.UseCases.LockAllTeams;

public class LockAllTeamsHandler
{
    private readonly ITeamRepository _repository;

    public LockAllTeamsHandler(ITeamRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle()
    {
        await _repository.LockAllTeamsAsync();
    }
}
