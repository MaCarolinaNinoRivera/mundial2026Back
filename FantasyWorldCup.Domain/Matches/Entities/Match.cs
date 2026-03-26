using FantasyWorldCup.Domain.Matches.Enums;
using FantasyWorldCup.Domain.Countries.Entities;

namespace FantasyWorldCup.Domain.Matches.Entities;

public class Match
{
    public Guid Id { get; private set; }
    public Guid HomeCountryId { get; private set; }
    public Guid AwayCountryId { get; private set; }
    public DateTime StartTime { get; private set; }
    public int? HomeGoals { get; private set; }
    public int? AwayGoals { get; private set; }
    public MatchStatus Status { get; private set; }

    public virtual Country HomeCountry { get; private set; } = null!;
    public virtual Country AwayCountry { get; private set; } = null!;

    private Match() { }

    public Match(Guid homeCountryId, Guid awayCountryId, DateTime startTime)
    {
        Id = Guid.NewGuid();
        HomeCountryId = homeCountryId;
        AwayCountryId = awayCountryId;
        StartTime = startTime;
        Status = MatchStatus.Scheduled;
    }

    public void SetResult(int homeGoals, int awayGoals)
    {
        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
        Status = MatchStatus.Finished;
    }

    public void UpdateScore(int homeGoals, int awayGoals)
    {
        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
    }

    public void UpdateStartTime(DateTime startTime)
    {
        StartTime = startTime;
    }

    public void UpdateStatus(MatchStatus status)
    {
        Status = status;
    }

}
