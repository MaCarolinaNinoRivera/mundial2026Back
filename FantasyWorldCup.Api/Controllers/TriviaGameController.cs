using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FantasyWorldCup.Application.Trivias.UseCases.GetNextQuestion;
using FantasyWorldCup.Application.Trivias.UseCases.AnswerQuestion;

namespace FantasyWorldCup.Api.Controllers;

[ApiController]
[Route("api/trivia-game")]
[Authorize]
public class TriviaGameController : ControllerBase
{
    [HttpGet("next")]
    public async Task<IActionResult> GetNext([FromServices] GetNextQuestionHandler handler)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var question = await handler.Handle(userId);

        if (question == null) return NotFound("¡Felicidades! Has respondido todas las preguntas.");

        return Ok(new
        {
            question.Id,
            question.QuestionText,
            question.OptionA,
            question.OptionB,
            question.OptionC,
            question.PointsValue
        });
    }

    [HttpPost("answer")]
    public async Task<IActionResult> Answer(
    [FromServices] AnswerQuestionHandler handler,
    [FromBody] AnswerQuestionCommand command)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await handler.Handle(userId, command);
        return Ok(result);
    }
}
