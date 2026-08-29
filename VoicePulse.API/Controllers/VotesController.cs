using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using VoicePulse.API.Extensions;
using VoicePulse.Application.Common.Consts;
using VoicePulse.Application.Contracts.Votes;
using VoicePulse.Application.Interfaces;
using VoicePulse.Application.Services;

namespace VoicePulse.API.Controllers;

[Route("api/polls/{pollId}/vote")]
[ApiController]
[Authorize(Roles = DefaultRoles.Member)]
[EnableRateLimiting(RateLimiters.Concurrency)]
public class VotesController(IQuestionService questionService , IVoteService voteService ) : ControllerBase
{
    private readonly IQuestionService _questionservice = questionService;
    private readonly IVoteService _voteservice = voteService;

    [HttpGet("")]
    public async Task<IActionResult> Start([FromRoute] int pollId , CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await _questionservice.GetAvaliableAsync(pollId, userId! ,cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequest request, CancellationToken cancellationToken)
    {
        var result = await _voteservice.AddAsync(pollId, User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess 
            ? Created() 
            : result.ToProblem();
    }
}
