namespace VoicePulse.Application.Contracts.Polls;

public record PollRequest(
    string Title,
    string Summary,
    DateOnly StartsAt,
    DateOnly EndsAt
);

