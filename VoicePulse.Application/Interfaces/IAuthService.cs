using VoicePulse.Application.Contracts.Authentication;

namespace VoicePulse.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> GetTokenAsync(string email, string password , CancellationToken cancellationToken = default);
}
