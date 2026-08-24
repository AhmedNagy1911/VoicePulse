using Microsoft.EntityFrameworkCore;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Poll> Polls { get;}
    DbSet<Question> Questions { get;}
    DbSet<Answer> Answers { get;}
     DbSet<Vote> Votes { get;}
     DbSet<VoteAnswer> VoteAnswers { get;}

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
