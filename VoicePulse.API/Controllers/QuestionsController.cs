using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoicePulse.API.Extensions;
using VoicePulse.Application.Contracts.Questions;
using VoicePulse.Application.Interfaces;
using VoicePulse.Application.Services;

namespace VoicePulse.API.Controllers;

[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionservice = questionService;

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
    {
        return Ok();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromRoute] int pollId , QuestionRequest request , CancellationToken cancellationToken)
    {
        var result = await _questionservice.AddAsync(pollId,request,cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { pollId, result.Value.Id }, result.Value)
            : result.ToProblem();
    }
}
