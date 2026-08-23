using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Answers;
using VoicePulse.Application.Contracts.Questions;

namespace VoicePulse.Application.Interfaces;

public interface IQuestionService
{
    Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId , CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> AddAsync(int pollId , QuestionRequest request  , CancellationToken cancellationToken = default);

    Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default);

}
