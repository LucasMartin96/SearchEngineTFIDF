namespace SearchEngine.Persistence.Context;

using Microsoft.EntityFrameworkCore;
using Core.Entities;

public class SearchEngineContext : DbContext
{
    public SearchEngineContext(DbContextOptions<SearchEngineContext> options) : base(options)
    {
    }

    public DbSet<Document> Documents { get; set; } = null!;
    public DbSet<Term> Terms { get; set; } = null!;
    public DbSet<TermOccurrence> TermOccurrences { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SearchEngineContext).Assembly);
    }
}