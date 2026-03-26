namespace FantasyWorldCup.Application.Matches.UseCases.RecordMatchStats;

// Clase DTO que recibe los datos
public class PlayerStatEntry
{
    public Guid FootballPlayerId { get; set; }
    public int MinutesPlayed { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public bool CleanSheet { get; set; }
    public int PenaltySaved { get; set; }
}

// El record para el Command
public record RecordMatchStatsCommand(
    Guid MatchId,
    List<PlayerStatEntry> Stats
);
