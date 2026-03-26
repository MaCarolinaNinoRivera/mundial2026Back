using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Domain.Matches.Entities;

namespace FantasyWorldCup.Application.Matches.UseCases.UpdateMatch;

public class UpdateMatchHandler
{
    private readonly IMatchRepository _repository;

    public UpdateMatchHandler(IMatchRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(Guid id, UpdateMatchCommand command)
    {
        var match = await _repository.GetByIdAsync(id);

        if (match is null)
            throw new KeyNotFoundException("Match not found");

        match.UpdateScore(command.HomeGoals, command.AwayGoals);
        match.UpdateStartTime(command.startTime);
        match.UpdateStatus(command.Status);

        await _repository.SaveChangesAsync();
    }
}
