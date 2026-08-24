namespace VoicePulse.Application.Common.Errors;

public static class UserErrors
{
     public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email/password" ,401);

    public static readonly Error InvalidJwtToken =
     new("User.InvalidJwtToken", "Invalid Jwt token" ,401);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Invalid refresh token",401);

}
