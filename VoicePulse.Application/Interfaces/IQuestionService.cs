using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Answers;
using VoicePulse.Application.Contracts.Questions;

namespace VoicePulse.Application.Interfaces;

public interface IQuestionService
{
    Task<Result<QuestionResponse>> AddAsync(int pollId , QuestionRequest request  , CancellationToken cancellationToken = default);
}
