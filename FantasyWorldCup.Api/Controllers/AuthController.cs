using Microsoft.AspNetCore.Mvc;
using FantasyWorldCup.Application.Auth.UseCases.Login;
using FantasyWorldCup.Application.Auth.UseCases.Register;
using FantasyWorldCup.Application.Auth.UseCases.RefreshToken;
using FantasyWorldCup.Application.Auth.UseCases.Logout;

namespace FantasyWorldCup.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromServices] RefreshTokenHandler handler,
        [FromBody] RefreshRequest request)
    {
        var result = await handler.Handle(request.RefreshToken);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromServices] RegisterUserHandler handler,
        [FromBody] RegisterUserCommand command)
    {
        var id = await handler.Handle(command);
        return Ok(id);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromServices] LoginHandler handler,
        [FromBody] LoginCommand command)
    {
        var result = await handler.Handle(command);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
    [FromServices] LogoutHandler handler,
    [FromBody] LogoutRequest request)
    {
        await handler.Handle(request.RefreshToken);
        return NoContent();
    }
}
