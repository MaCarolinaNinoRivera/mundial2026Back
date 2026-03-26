using FantasyWorldCup.Domain.Matches.Enums;
namespace FantasyWorldCup.Application.Matches.UseCases.UpdateMatch;

public class UpdateMatchCommand
{
    public int HomeGoals { get; set; }
    public int AwayGoals { get; set; }
    public MatchStatus Status { get; set; }
    public DateTime startTime { get; set; }
}
