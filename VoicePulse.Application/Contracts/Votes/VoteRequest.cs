namespace VoicePulse.Application.Contracts.Votes;

public record VoteRequest(
    IEnumerable<VoteAnswerRequest> Answers
);