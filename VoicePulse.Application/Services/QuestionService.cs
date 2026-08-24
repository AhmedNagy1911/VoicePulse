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

    public async  Task<Result<IEnumerable<QuestionResponse>>> GetAvaliableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
    {
        var isVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken:cancellationToken);

        if (isVote)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken: cancellationToken);
         
        if(!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions =await _context.Questions
                .Where(x => x.PollId == pollId && x.IsActive)
                .Include(x => x.Answers)
                .Select(q => new QuestionResponse(
                q.Id,
                q.Content,
                q.Answers.Where(q => q.IsActive).Select(a => new Contracts.Answers.AnswerResponse(a.Id ,a.Content))
                ))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }

    public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var questions = await _context.Questions
              .Where(x => x.PollId == pollId && x.Id == id)
              .Include(x => x.Answers)
              .ProjectToType<QuestionResponse>()
              .AsNoTracking()
              .SingleOrDefaultAsync(cancellationToken: cancellationToken);

        if (questions is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        return Result.Success(questions);

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
    public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var questionIsExists = await _context.Questions
           .AnyAsync(x => x.PollId == pollId
               && x.Id != id
               && x.Content == request.Content,
               cancellationToken
           );

        if (questionIsExists)
            return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

        var question = await _context.Questions
            .Include(x => x.Answers)
            .SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        //current answers
        var currentAnswers = question.Answers.Select(x => x.Content).ToList();

        //add new answer
        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        newAnswers.ForEach(answer =>
        {
            question.Answers.Add(new Answer { Content = answer });
        });

        question.Answers.ToList().ForEach(answer =>
        {
            answer.IsActive = request.Answers.Contains(answer.Content);
        });

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }

    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    } 
}
