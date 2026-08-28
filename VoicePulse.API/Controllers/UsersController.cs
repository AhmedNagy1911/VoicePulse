using Microsoft.AspNetCore.Mvc;
using VoicePulse.Application.Interfaces;

namespace VoicePulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

}
