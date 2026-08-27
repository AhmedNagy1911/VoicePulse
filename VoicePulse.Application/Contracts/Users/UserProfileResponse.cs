namespace VoicePulse.Application.Contracts.Users;

public record UserProfileResponse(
    string Email,
    string UserName,
    string FristName,
    string LastName
);