using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FantasyWorldCup.Application.Teams.UseCases.BuyPlayer;
using FantasyWorldCup.Application.Teams.UseCases.SellPlayer;
using Microsoft.EntityFrameworkCore;
using FantasyWorldCup.Infrastructure.Persistence;
using FantasyWorldCup.Domain.Teams.Entities;
using FantasyWorldCup.Application.Teams.Queries.GetMyTeam; // Importa el DTO
using FantasyWorldCup.Application.Teams.UseCases.SetLineup;
using FantasyWorldCup.Application.Teams.UseCases.MakeSubstitution;
using FantasyWorldCup.Application.Teams.Interfaces;
using FantasyWorldCup.Application.Teams.Queries.GetMatchLineup;
using FantasyWorldCup.Application.Teams.Queries.GetAvailableBench;
using FantasyWorldCup.Application.Teams.UseCases.UpdateTeamKit;

namespace FantasyWorldCup.Api.Controllers;

[ApiController]
[Route("api/user-team")]
[Authorize]
public class UserTeamController : ControllerBase
{
    private readonly BuyPlayerHandler _buyHandler;
    private readonly SellPlayerHandler _sellHandler;
    private readonly AppDbContext _context;

    public UserTeamController(BuyPlayerHandler buyHandler, SellPlayerHandler sellHandler, AppDbContext context)
    {
        _buyHandler = buyHandler;
        _sellHandler = sellHandler;
        _context = context;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyTeam([FromServices] GetMyTeamHandler handler)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // El controlador solo conoce el Handler y el DTO
        var result = await handler.Handle(new GetMyTeamQuery(userId));

        return Ok(result);
    }

    [HttpPost("buy")]
    public async Task<IActionResult> BuyPlayer([FromBody] BuyPlayerCommand command)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _buyHandler.Handle(userId, command);
        return Ok(new { message = "Jugador comprado exitosamente." });
    }

    [HttpDelete("remove-player/{playerId}")]
    public async Task<IActionResult> RemovePlayer(Guid playerId)
    {
        // Obtenemos el UserId del token JWT que ya tienes configurado
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var command = new SellPlayerCommand(userId, playerId);
        await _sellHandler.Handle(command);

        return Ok(new { message = "Jugador vendido y dinero reembolsado." });
    }

    [HttpPost("lineup")]
    public async Task<IActionResult> SetLineup([FromServices] SetLineupHandler handler, [FromBody] SetLineupCommand command)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await handler.Handle(userId, command);
        return Ok("Alineación confirmada. ¡Mucha suerte!");
    }

    [HttpGet("lineup/{matchId}")]
    public async Task<IActionResult> GetLineup(
    [FromServices] GetMatchLineupHandler handler,
    Guid matchId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await handler.Handle(userId, matchId);
        return Ok(result);
    }

    [HttpGet("bench/{matchId}")]
    public async Task<IActionResult> GetBench([FromServices] GetAvailableBenchHandler handler, Guid matchId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await handler.Handle(userId, matchId);
        return Ok(result);
    }

    [HttpPost("substitute")]
    public async Task<IActionResult> Substitute(
    [FromServices] MakeSubstitutionHandler handler,
    [FromBody] MakeSubstitutionCommand command)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await handler.Handle(userId, command.MatchId, command.PlayerOutId, command.PlayerInId);
        return Ok(new { message = "Cambio realizado con éxito." });
    }

    [HttpPut("my/kit")] // Usamos PUT porque estamos actualizando un recurso
    public async Task<IActionResult> UpdateKit([FromBody] UpdateKitRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var handler = HttpContext.RequestServices.GetRequiredService<UpdateTeamKitHandler>();

        await handler.ExecuteAsync(new UpdateTeamKitCommand(
            userId,
            request.PrimaryColor,
            request.SecondaryColor
        ));

        return Ok(new { message = "Uniforme actualizado con éxito." });
    }

    // Clase de apoyo para el Body
    public record UpdateKitRequest(string PrimaryColor, string SecondaryColor);
}
