namespace VoicePulse.Application.Common.Errors;

public record PollErrors
{
    public static readonly Error PollNotFound =
        new("Poll.NotFound", "No poll was found with the given ID" ,404);

    public static readonly Error DuplicatedTitle =
        new("Poll.Duplicated", "another poll with the same poll already exists" ,409);
}