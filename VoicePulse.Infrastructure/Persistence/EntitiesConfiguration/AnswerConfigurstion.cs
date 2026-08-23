using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Infrastructure.Persistence.EntitiesConfiguration;

public class AnswerConfigurstion : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasIndex(x => new { x.QuestionId, x.Content }).IsUnique();

        builder.Property(x => x.Content).HasMaxLength(1000);
    }
}
