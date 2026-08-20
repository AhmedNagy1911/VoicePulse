using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Application.Services;

public class PollService : IPollService
{
    private static readonly List<Poll> _poll =
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
    public Poll Add(Poll poll)
    {
        poll.Id = _poll.Count + 1; // Simple ID generation
        _poll.Add(poll);

        return poll;
    }

    public bool Update(int id , Poll poll)
    {
        var existingPoll = GetById(id);
        if (existingPoll is null)
            return false;

        existingPoll.Title = poll.Title;
        existingPoll.Description = poll.Description;
        return true;
    }
    public bool Delete(int id)
    {
        var Poll = GetById(id);
        if (Poll is null)
            return false;

        _poll.Remove(Poll);
       
        return true;
    }
}
