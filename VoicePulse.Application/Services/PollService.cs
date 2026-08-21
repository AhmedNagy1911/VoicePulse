using Microsoft.EntityFrameworkCore;
using VoicePulse.Application.Common.Interfaces;
using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Application.Services;

public class PollService(IApplicationDbContext context) : IPollService
{
    private readonly IApplicationDbContext _context = context;

    public async Task<IEnumerable<Poll>> GetAllAsync() =>
        await _context.Polls.AsNoTracking().ToListAsync();


    public async Task<Poll?> GetByIdAsync(int id) =>
        await _context.Polls.FindAsync(id);

    public async Task<Poll> AddAsync(Poll poll)
    {
        await _context.Polls.AddAsync(poll);
        await _context.SaveChangesAsync();

        return poll;
    }

    //public bool Update(int id , Poll poll)
    //{
    //    var existingPoll = GetById(id);
    //    if (existingPoll is null)
    //        return false;

    //    existingPoll.Title = poll.Title;
    //    existingPoll.Summary = poll.Summary;
    //    return true;
    //}
    //public bool Delete(int id)
    //{
    //    var Poll = GetById(id);
    //    if (Poll is null)
    //        return false;

    //    _poll.Remove(Poll);

    //    return true;
    //}
}
