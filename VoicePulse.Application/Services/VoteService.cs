using Mapster;
using Microsoft.EntityFrameworkCore;
using VoicePulse.Application.Common.Errors;
using VoicePulse.Application.Common.Interfaces;
using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Votes;
using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Application.Services;

public class VoteService(IApplicationDbContext context) : IVoteService
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
    {
        var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure(VoteErrors.DuplicatedVote);

        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!pollIsExists)
            return Result.Failure(PollErrors.PollNotFound);

        var availableQuestions = await _context.Questions
           .Where(x => x.PollId == pollId && x.IsActive)
           .Select(x => x.Id)
           .ToListAsync(cancellationToken);

        if (!request.Answers.Select(x => x.QuestionId).SequenceEqual(availableQuestions))
            return Result.Failure(VoteErrors.InvalidQuestions);

        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };

        await _context.Votes.AddAsync(vote, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
}
