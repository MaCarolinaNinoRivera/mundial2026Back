namespace FantasyWorldCup.Application.Matches.Queries.GetMatchPlayers;

public class MatchPlayersDto
{
    public Guid PlayerId { get; set; }
    public string Name { get; set; } = null!;
    public string Position { get; set; } = null!;
    public string CountryName { get; set; } = null!;
    public bool IsHomeTeam { get; set; } // Para separar locales de visitantes en la UI
}
