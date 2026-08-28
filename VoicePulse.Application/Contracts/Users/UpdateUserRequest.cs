namespace VoicePulse.Application.Contracts.Users;

public record UpdateUserRequest(
    string FristName,
    string LastName,
    string Email,
    string UserName,
    IList<string> Roles
);