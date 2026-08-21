using Microsoft.EntityFrameworkCore;
using System.Reflection;
using VoicePulse.Application.Common.Interfaces;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options) , IApplicationDbContext
{
    public DbSet<Poll> Polls { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
