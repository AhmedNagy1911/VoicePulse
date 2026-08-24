using Microsoft.EntityFrameworkCore;
using VoicePulse.Application.Common.Errors;
using VoicePulse.Application.Common.Interfaces;
using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Results;
using VoicePulse.Application.Interfaces;

namespace VoicePulse.Application.Services;

public class ResultService(IApplicationDbContext context) : IResultService
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
    {
       var pollVotes = await _context.Polls
            .Where(x => x.Id == pollId)
            .Select(x => new PollVotesResponse(
                x.Title,
                x.Votes.Select(v => new VoteResponse(
                 $"{v.User.FristName} {v.User.LastName}",
                    v.SubmittedOn,
                    v.VoteAnswers.Select(a => new QuestionAnswerResponse(
                       a.Question.Content,
                       a.Answer.Content
                    ))  
                ))
            ))
            .SingleOrDefaultAsync(cancellationToken);

        return pollVotes is not null
            ? Result.Success(pollVotes)
            : Result.Failure<PollVotesResponse>(PollErrors.PollNotFound);
    }
}
