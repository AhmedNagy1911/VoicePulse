using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoicePulse.API.Extensions;
using VoicePulse.Application.Interfaces;

namespace VoicePulse.API.Controllers;

[Route("me")]
[ApiController]
[Authorize]
public class AccountController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userservice = userService;

    [HttpGet("")]
    public async Task<IActionResult> Info()
    {
        var result = await _userservice.GetProfileAsync(User.GetUserId()!);

        return Ok(result.Value);
    }


}
