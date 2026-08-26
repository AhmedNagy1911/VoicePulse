namespace VoicePulse.Application.Contracts.Authentication;

 public record RegisterRequest(
    string Email,
    string Password,
    string UserName,
    string FristName,
    string LastName
);