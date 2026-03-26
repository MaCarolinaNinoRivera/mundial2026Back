namespace FantasyWorldCup.Application.Teams.UseCases.SetLineup;

public record SetLineupCommand(
    Guid MatchId,
    List<Guid> PlayerIds // Lista de IDs de FootballPlayer (o UserTeamPlayer) que serán titulares
);
