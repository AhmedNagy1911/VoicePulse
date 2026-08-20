using Microsoft.AspNetCore.Mvc;
using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;

namespace VoicePulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollservice = pollService;

    [HttpGet("")]
    public IActionResult GetAll()
    {
        return Ok(_pollservice.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var poll = _pollservice.GetById(id);    

        return poll is not null ? Ok(poll) : NotFound();
    }

    [HttpPost("")]
    public IActionResult Add(Poll poll)
    {
        var newPoll = _pollservice.Add(poll);

        return CreatedAtAction(nameof(GetById), new { id = newPoll.Id }, newPoll);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Poll poll)
    {
        var isUpdated = _pollservice.Update(id,poll);

        return isUpdated ? NoContent() : NotFound();
    }
    [HttpDelete("{id}")]
    public IActionResult Delete (int id)
    {
        var isDelete = _pollservice.Delete(id);

        return isDelete ? NoContent() : NotFound();
    }
}
