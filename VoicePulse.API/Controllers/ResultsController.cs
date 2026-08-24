using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoicePulse.API.Extensions;
using VoicePulse.Application.Interfaces;
using VoicePulse.Application.Services;

namespace VoicePulse.API.Controllers;

[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class ResultsController(IResultService resultService ) : ControllerBase
{
    private readonly IResultService _resultservice = resultService;

    [HttpGet("row-data")]
    public async Task<IActionResult> PollVotes([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _resultservice.GetPollVotesAsync(pollId, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : result.ToProblem();
    }

    [HttpGet("votes-per-day")]
    public async Task<IActionResult> VotesPerDay([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _resultservice.GetVotesPerDayAsync(pollId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("votes-per-question")]
    public async Task<IActionResult> VotesPerQuestion([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _resultservice.GetVotesPerQuestionAsync(pollId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
