using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VoicePulse.API.Extensions;
using VoicePulse.API.Filters;
using VoicePulse.Application.Common.Consts;
using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Polls;
using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;

namespace VoicePulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]

public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollservice = pollService;

    [HttpGet("")]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        return Ok(await _pollservice.GetAllAsync(cancellationToken));
    }

    [HttpGet("current")]
    [Authorize(Roles = DefaultRoles.Member)]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> GetCurrentV1(CancellationToken cancellationToken = default)
    {
        return Ok(await _pollservice.GetCurrentAsyncV1(cancellationToken));
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _pollservice.GetAsync(id, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            :result.ToProblem();
    }

    [HttpPost("")]
    [HasPermission(Permissions.AddPolls)]
    public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _pollservice.AddAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Adapt<PollResponse>())
            :result.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _pollservice.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess 
            ? NoContent() 
            : result.ToProblem();
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.DeletePolls)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _pollservice.DeleteAsync(id, cancellationToken);

        return result.IsSuccess 
            ? NoContent() 
            : result.ToProblem();
    }

    [HttpPut("{id}/togglePublish")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _pollservice.TogglePublishStatusAsync(id, cancellationToken);

        return result.IsSuccess 
            ? NoContent() 
            : result.ToProblem();
    }
}
