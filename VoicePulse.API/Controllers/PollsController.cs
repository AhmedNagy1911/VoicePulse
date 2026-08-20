using Microsoft.AspNetCore.Mvc;
using VoicePulse.Domain.Entities;

namespace VoicePulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController : ControllerBase
{
    private readonly List<Poll> _poll = 
        [
        new Poll{
            Id = 1,
            Title = "Poll 1",
            Description = "Description for Poll 1"
        }

        ];

    [HttpGet("")]
    public IActionResult GetAll()
    {
        return Ok(_poll);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var poll = _poll.SingleOrDefault(p => p.Id == id);
      
        return poll is not null ? Ok(poll) : NotFound();
    }
}
