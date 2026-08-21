namespace VoicePulse.Application.Contracts.Polls;

public record PollResponse(
    int Id,
    string Title,
    string Description
);
