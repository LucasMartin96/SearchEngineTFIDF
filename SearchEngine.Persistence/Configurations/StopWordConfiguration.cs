namespace SearchEngine.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;

public class StopWordConfiguration : IEntityTypeConfiguration<StopWord>
{
    public void Configure(EntityTypeBuilder<StopWord> builder)
    {
        builder.ToTable("StopWords");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Word)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Language)
            .IsRequired()
            .HasMaxLength(2);

        builder.HasIndex(x => new { x.Word, x.Language })
            .IsUnique();
    }
}