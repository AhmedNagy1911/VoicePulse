using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Infrastructure.Persistence.EntitiesConfiguration;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasIndex(x => new { x.PollId, x.Content }).IsUnique();

        builder.Property(x => x.Content).HasMaxLength(1000);
    }
}
