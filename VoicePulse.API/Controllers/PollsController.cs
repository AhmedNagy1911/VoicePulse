using Mapster;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var polls = await _pollservice.GetAllAsync(cancellationToken);
         
        var response = polls.Adapt<IEnumerable<PollResponse>>();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var poll = await _pollservice.GetByIdAsync(id, cancellationToken);

        var response = poll.Adapt<PollResponse>();

        return poll is not null ? Ok(response) : NotFound();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest poll , CancellationToken cancellationToken = default)
    {
        var newPoll = await _pollservice.AddAsync(poll.Adapt<Poll>(), cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = newPoll.Id }, newPoll);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest poll, CancellationToken cancellationToken = default)
    {
        var isUpdated = await _pollservice.UpdateAsync(id, poll.Adapt<Poll>(), cancellationToken);

        return isUpdated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var isDelete = await _pollservice.DeleteAsync(id, cancellationToken);

        return isDelete ? NoContent() : NotFound();
    }

    [HttpPut("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var isUpdated = await _pollservice.TogglePublishStatusAsync(id, cancellationToken);

        return isUpdated ? NoContent() : NotFound();
    }
}
