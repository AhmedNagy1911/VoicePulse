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
    public async Task<IActionResult> GetAll()
    {
        var polls = await _pollservice.GetAllAsync();
         
        var response = polls.Adapt<IEnumerable<PollResponse>>();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var poll = await _pollservice.GetByIdAsync(id);

        var response = poll.Adapt<PollResponse>();

        return poll is not null ? Ok(response) : NotFound();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest poll)
    {
        var newPoll = await _pollservice.AddAsync(poll.Adapt<Poll>());

        return CreatedAtAction(nameof(GetById), new { id = newPoll.Id }, newPoll);
    }

    //[HttpPut("{id}")]
    //public IActionResult Update([FromRoute] int id, [FromBody] PollRequest poll)
    //{
    //    var isUpdated = _pollservice.Update(id , poll.Adapt<Poll>());

    //    return isUpdated ? NoContent() : NotFound();
    //}
    //[HttpDelete("{id}")]
    //public IActionResult Delete ([FromRoute] int id)
    //{
    //    var isDelete = _pollservice.Delete(id);

    //    return isDelete ? NoContent() : NotFound();
    //}
}
