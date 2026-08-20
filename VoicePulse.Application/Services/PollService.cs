using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Application.Services;

public class PollService : IPollService
{
    private readonly List<Poll> _poll =
   [
           new Poll{
                Id = 1,
                Title = "Poll 1",
                Description = "Description for Poll 1"
            }
   ]; 
    public IEnumerable<Poll> GetAll()
    {
        return _poll; 
    }

    public Poll? GetById(int id)
    {
        return _poll.SingleOrDefault(p => p.Id == id);
    }
}
