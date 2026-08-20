using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VoicePulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ValuesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new string[] { "value1", "value2" });
    }
}
