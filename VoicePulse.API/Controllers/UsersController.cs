using Microsoft.AspNetCore.Mvc;
using VoicePulse.API.Filters;
using VoicePulse.Application.Common.Consts;
using VoicePulse.Application.Interfaces;

namespace VoicePulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    [HttpGet("")]
    [HasPermission(Permissions.GetUsers)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _userService.GetAllAsync(cancellationToken));
    }

}
