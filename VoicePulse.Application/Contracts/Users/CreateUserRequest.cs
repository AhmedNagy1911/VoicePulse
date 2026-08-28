namespace VoicePulse.Application.Contracts.Users;

public record CreateUserRequest(
    string FristName,
    string LastName,
    string Email,
    string UserName,
    string Password,
    IList<string> Roles
);