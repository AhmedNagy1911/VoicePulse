using Mapster;
using Microsoft.EntityFrameworkCore;
using VoicePulse.Application.Common.Errors;
using VoicePulse.Application.Common.Interfaces;
using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Questions;
using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Application.Services;

public class QuestionService(IApplicationDbContext context) : IQuestionService
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken  = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _context.Questions
              .Where(x=> x.PollId == pollId)
              .Include(x => x.Answers)
              //.Select(q => new QuestionResponse(
              //    q.Id,
              //    q.Content,
              //    q.Answers.Select(a => new AnswerResponse(a.Id, a.Content))
              //))
              .ProjectToType<QuestionResponse>()
              .AsNoTracking()
              .ToListAsync(cancellationToken: cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }
    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);

        if (!pollIsExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionIsExists = await _context.Questions.AnyAsync(x => x.Content == request.Content && x.PollId == pollId, cancellationToken: cancellationToken);

        if(questionIsExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

        var question = request.Adapt<Question>();
        question.PollId = pollId;

        await _context.Questions.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());

    }
}
