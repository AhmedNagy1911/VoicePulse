using VoicePulse.Domain.Entities;

namespace VoicePulse.Application.Interfaces;

public interface IPollService
{
    IEnumerable<Poll> GetAll();
    Poll? GetById(int id);
    Poll Add(Poll poll);
}
