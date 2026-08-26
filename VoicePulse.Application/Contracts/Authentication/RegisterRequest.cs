namespace VoicePulse.Application.Contracts.Authentication;

 public record RegisterRequest(
    string Email,
    string Password,
    string UserName,
    string FirstName,
    string LastName
);