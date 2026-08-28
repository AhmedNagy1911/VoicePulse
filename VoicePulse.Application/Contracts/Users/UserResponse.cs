namespace VoicePulse.Application.Contracts.Users;

public record UserResponse(
    string Id,
    string FristName,
    string LastName,
    string Email,
    string UserName,
    bool IsDisabled,
    IEnumerable<string> Roles
);