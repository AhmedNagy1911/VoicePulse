using VoicePulse.Application.Common.Models;
using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Answers;
using VoicePulse.Application.Contracts.Questions;

namespace VoicePulse.Application.Interfaces;

public interface IQuestionService
{
    Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId, RequestFilters filters, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QuestionResponse>>> GetAvaliableAsync(int pollId ,string userId , CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> AddAsync(int pollId , QuestionRequest request  , CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default);

}
