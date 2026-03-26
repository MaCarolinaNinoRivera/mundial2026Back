namespace FantasyWorldCup.Application.Teams.UseCases.UpdateTeamKit;

public record UpdateTeamKitCommand(
    Guid UserId,
    string PrimaryColor,
    string SecondaryColor
);
