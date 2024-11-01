namespace SearchEngine.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;

public class TermConfiguration : IEntityTypeConfiguration<Term>
{
    public void Configure(EntityTypeBuilder<Term> builder)
    {
        builder.ToTable("Terms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Word)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DocumentCount)
            .IsRequired();

        builder.HasMany(x => x.Occurrences)
            .WithOne(x => x.Term)
            .HasForeignKey(x => x.TermId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Word)
            .IsUnique();
    }
}