namespace VoicePulse.Application.Contracts.Authentication;

public record AuthResponse(
    string Id,
    string FirstName,
    string LastName,
    string? Email,
    string Token,
    int ExpiresIn
);

