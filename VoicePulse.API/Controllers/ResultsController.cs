using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoicePulse.API.Extensions;
using VoicePulse.Application.Interfaces;

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
      
}
