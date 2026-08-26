namespace VoicePulse.Application.Contracts.Authentication;

public record AuthResponse(
    string Id,
    string? Email,
    string UserName,
    string FirstName,
    string LastName,
    string Token,
    int ExpiresIn,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);

