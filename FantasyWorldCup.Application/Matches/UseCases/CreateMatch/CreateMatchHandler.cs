using FantasyWorldCup.Application.Matches.Interfaces;
using FantasyWorldCup.Domain.Matches.Entities;
using FantasyWorldCup.Application.Countries.Interfaces;

namespace FantasyWorldCup.Application.Matches.UseCases.CreateMatch;

public class CreateMatchHandler
{
    private readonly IMatchRepository _matchRepository;
    private readonly ICountryRepository _countryRepository;

    public CreateMatchHandler(
        IMatchRepository matchRepository,
        ICountryRepository countryRepository)
    {
        _matchRepository = matchRepository;
        _countryRepository = countryRepository;
    }

    public async Task<Guid> Handle(CreateMatchCommand command)
    {
        if (!await _countryRepository.ExistsAsync(command.HomeCountryId))
            throw new KeyNotFoundException("Home country not found");

        if (!await _countryRepository.ExistsAsync(command.AwayCountryId))
            throw new KeyNotFoundException("Away country not found");

        if (command.HomeCountryId == command.AwayCountryId)
            throw new ArgumentException("Home and Away country cannot be the same");

        var match = new Match(
            command.HomeCountryId,
            command.AwayCountryId,
            command.StartTime
        );

        await _matchRepository.AddAsync(match);
        await _matchRepository.SaveChangesAsync();

        return match.Id;
    }
}
