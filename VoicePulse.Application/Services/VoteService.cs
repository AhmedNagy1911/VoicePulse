using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Votes;
using VoicePulse.Application.Interfaces;

namespace VoicePulse.Application.Services;

public class VoteService : IVoteService
{
    public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
