using VoicePulse.Domain.Entities;

namespace VoicePulse.Application.Interfaces;

public interface IPollService
{
    Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Poll?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default);

    Task<bool> Update(int id, Poll poll , CancellationToken cancellationToken = default);
    Task<bool> Delete(int id , CancellationToken cancellationToken = default);
}
