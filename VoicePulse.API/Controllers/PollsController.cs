using Microsoft.AspNetCore.Mvc;
using VoicePulse.Domain.Entities;

namespace VoicePulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController : ControllerBase
{
    private readonly List<Poll> _poll = [];

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_poll);
    }
}
