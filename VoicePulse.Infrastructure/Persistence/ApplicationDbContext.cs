using Microsoft.EntityFrameworkCore;
using VoicePulse.Application.Common.Interfaces;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options) , IApplicationDbContext
{
    public DbSet<Poll> Polls { get; set; }
}
