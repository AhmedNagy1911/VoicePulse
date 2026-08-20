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
}
