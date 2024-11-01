namespace SearchEngine.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;

public class TermOccurrenceConfiguration : IEntityTypeConfiguration<TermOccurrence>
{
    public void Configure(EntityTypeBuilder<TermOccurrence> builder)
    {
        builder.ToTable("TermOccurrences");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Frequency)
            .IsRequired();

        builder.HasIndex(x => new { x.TermId, x.DocumentId })
            .IsUnique();
    }
}