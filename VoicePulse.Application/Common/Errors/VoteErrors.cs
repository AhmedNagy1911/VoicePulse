using Microsoft.AspNetCore.Http;

namespace VoicePulse.Application.Common.Errors;

public static class VoteErrors
{
    public static readonly Error InvalidQuestions =
        new("Vote.InvalidQuestions", "Invalid questions", 400);

    public static readonly Error DuplicatedVote =
        new("Vote.DuplicatedVote", "This user already voted before for this poll",409);
}