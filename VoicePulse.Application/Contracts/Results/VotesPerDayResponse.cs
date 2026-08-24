namespace VoicePulse.Application.Contracts.Results;

public record VotesPerDayResponse(
    DateOnly Date,
    int NumberOfVotes
);