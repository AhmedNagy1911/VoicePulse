namespace VoicePulse.Application.Contracts.Authentication;

public record RefreshTokenRequest(
    string Token, 
    string RefreshToken
);

