namespace VoicePulse.Application.Contracts.Users;

public record UserResponse(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    bool IsDisabled,
    IEnumerable<string> Roles
);