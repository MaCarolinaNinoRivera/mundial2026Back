namespace FantasyWorldCup.Application.Auth.UseCases.Login;

public record AuthResponse(
    string AccessToken,
    string RefreshToken
);
