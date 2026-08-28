using Microsoft.AspNetCore.Http;

namespace VoicePulse.Application.Common.Errors;

public static class UserErrors
{
     public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email/password" ,401);
    public static readonly Error DisabledUser =
        new("User.DisabledUser", "Disabled user, please contact your administrator", StatusCodes.Status401Unauthorized);

    public static readonly Error LockedUser =
        new("User.LockedUser", "Locked user, please contact your administrator", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidJwtToken =
     new("User.InvalidJwtToken", "Invalid Jwt token" ,401);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Invalid refresh token",401);

    public static readonly Error DuplicatedEmail =
      new("User.DuplicatedEmail", "Another user with the same email is already exists", 409);

    public static readonly Error EmailNotConfirmed =
        new("User.EmailNotConfirmed", "Email is not confirmed", 401);

    public static readonly Error InvalidCode =
        new("User.InvalidCode", "Invalid code", 401);

    public static readonly Error DuplicatedConfirmation =
        new("User.DuplicatedConfirmation", "Email already confirmed", 400);

}
