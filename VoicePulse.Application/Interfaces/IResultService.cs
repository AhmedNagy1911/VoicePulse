using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Results;

namespace VoicePulse.Application.Interfaces;

public interface IResultService
{
    Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default);

}
