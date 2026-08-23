using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;
using VoicePulse.Application.Common.Interfaces;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options , IHttpContextAccessor httpContextAccessor) 
    : IdentityDbContext<ApplicationUser>(options) , IApplicationDbContext
{
    private readonly IHttpContextAccessor _httpcontextaccessor = httpContextAccessor;

    public DbSet<Answer> Answers { get; set; }
    public DbSet<Poll> Polls { get; set; }
    public DbSet<Question> Questions  { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        //Change OnDelete Behavior
        var cascadeFKs = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;


        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();

        foreach (var entityEntry in entries) 
        {
            var currentUserId = _httpcontextaccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(x => x.CreatedById).CurrentValue = currentUserId;
            }
            else if (entityEntry.State == EntityState.Modified) 
            {
                entityEntry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
                entityEntry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

}
