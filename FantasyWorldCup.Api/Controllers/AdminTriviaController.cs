using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FantasyWorldCup.Application.Trivias.Interfaces;
using FantasyWorldCup.Application.Trivias.UseCases.CreateQuestion;
using FantasyWorldCup.Application.Trivias.UseCases.UpdateQuestion;

namespace FantasyWorldCup.Api.Controllers;

[ApiController]
[Route("api/admin/trivias")]
[Authorize(Roles = "Admin")]
public class AdminTriviaController : ControllerBase
{
    private readonly CreateQuestionHandler _createHandler;
    private readonly ITriviaRepository _repo;
    private readonly UpdateQuestionHandler _handler;

    public AdminTriviaController(CreateQuestionHandler createHandler, 
        ITriviaRepository repo,
        UpdateQuestionHandler updateHandler)
    {
        _createHandler = createHandler;
        _repo = repo;
        _handler = updateHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateQuestionCommand command)
    {
        var id = await _createHandler.Handle(command);
        return Ok(id);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var q = await _repo.GetByIdAsync(id);
        if (q == null) return NotFound();
        _repo.Remove(q);
        await _repo.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionCommand command)
    {
        await _handler.Handle(id, command);
        return NoContent();
    }
}