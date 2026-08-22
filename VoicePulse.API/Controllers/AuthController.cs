using Microsoft.AspNetCore.Mvc;
using VoicePulse.Application.Contracts.Authentication;
using VoicePulse.Application.Interfaces;

namespace VoicePulse.API.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authservice = authService;

    [HttpPost("")]
    public async Task<IActionResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var authResult = await _authservice.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return authResult is null ? BadRequest("Invalid email/password") : Ok(authResult);
    }
}
