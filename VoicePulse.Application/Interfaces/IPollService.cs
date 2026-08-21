using VoicePulse.Domain.Entities;

namespace VoicePulse.Application.Interfaces;

public interface IPollService
{
    Task<IEnumerable<Poll>> GetAllAsync();
    Task<Poll?> GetByIdAsync(int id);
    Task<Poll> AddAsync(Poll poll);
    //Task<bool> Update(int id, Poll poll);
    //Task<bool> Delete(int id);
}
