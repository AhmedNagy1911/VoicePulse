namespace VoicePulse.Application.Common.Errors;

public static class UserErrors
{
     public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email/password");

}
