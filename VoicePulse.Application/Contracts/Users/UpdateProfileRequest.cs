namespace VoicePulse.Application.Contracts.Users;

public record UpdateProfileRequest(
    string FristName,
    string LastName
);