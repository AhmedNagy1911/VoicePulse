using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Votes;

namespace VoicePulse.Application.Interfaces;

public interface IVoteService
{
    Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default);
}