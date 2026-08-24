using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoicePulse.API.Extensions;
using VoicePulse.Application.Interfaces;

namespace VoicePulse.API.Controllers;

[Route("api/polls/{pollId}/vote")]
[ApiController]
[Authorize]
public class VotesController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionservice = questionService;

    [HttpGet("")]
    public async Task<IActionResult> Start([FromRoute] int pollId , CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await _questionservice.GetAvaliableAsync(pollId, userId! ,cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Value)
            : result.ToProblem();
    }
}
