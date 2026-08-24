namespace VoicePulse.Application.Contracts.Results;

public record VotesPerAnswerResponse(
    string Answer,
    int Count
);