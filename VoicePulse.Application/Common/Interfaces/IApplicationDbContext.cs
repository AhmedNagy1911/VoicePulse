using Microsoft.EntityFrameworkCore;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Poll> Polls { get;}

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
