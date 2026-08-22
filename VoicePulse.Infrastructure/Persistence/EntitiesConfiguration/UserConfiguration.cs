using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Infrastructure.Persistence.EntitiesConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FristName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);
    }
}
