namespace SearchEngine.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Core.Entities;
using SearchEngine.Core.Interfaces.Repositories;
using Context;

public class TermOccurrenceRepository : BaseRepository<TermOccurrence>, ITermOccurrenceRepository
{
    private readonly ISearchEngineContextFactory _contextFactory;

    public TermOccurrenceRepository(SearchEngineContext context, ISearchEngineContextFactory contextFactory) 
        : base(context)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<Dictionary<(Guid TermId, Guid DocumentId), double>> GetTermFrequenciesAsync(
        List<Guid> termIds, 
        Guid documentId)
    {
        using SearchEngineContext context = _contextFactory.CreateDbContext();
        List<TermOccurrence> occurrences = await context.Set<TermOccurrence>()
            .Where(x => termIds.Contains(x.TermId) && x.DocumentId == documentId)
            .ToListAsync();

        return occurrences.ToDictionary<TermOccurrence, (Guid TermId, Guid DocumentId), double>(
            x => (x.TermId, x.DocumentId),
            x => x.Frequency
        );
    }

    public override async Task<int> GetTotalCountAsync()
    {
        using SearchEngineContext context = _contextFactory.CreateDbContext();
        return await context.Set<TermOccurrence>()
            .AsNoTracking()
            .SumAsync(to => to.Frequency);
    }
}