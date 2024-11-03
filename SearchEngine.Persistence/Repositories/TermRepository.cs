namespace SearchEngine.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Core.Entities;
using SearchEngine.Core.Interfaces.Repositories;
using Context;

// TODO: Just bring it without tracking 
public class TermRepository : BaseRepository<Term>, ITermRepository
{
    private readonly ISearchEngineContextFactory _contextFactory;

    public TermRepository(SearchEngineContext context, ISearchEngineContextFactory contextFactory) 
        : base(context)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Term?> GetByWordAsync(string word)
    {
        return await Entities
            .FirstOrDefaultAsync(x => x.Word == word);
    }

    public async Task<List<Term>> GetByWordsAsync(List<string> words)
    {
        return await Entities
            .Where(x => words.Contains(x.Word))
            .ToListAsync();
    }

    public async Task<Dictionary<string, double>> GetInverseDocumentFrequenciesAsync(List<string> words)
    {
        using SearchEngineContext context = _contextFactory.CreateDbContext();
        int totalDocuments = await context.Documents.CountAsync();
        
        var termCounts = await context.Set<Term>()
            .Where(x => words.Contains(x.Word))
            .Select(x => new { x.Word, x.DocumentCount })
            .ToListAsync();

        return termCounts.ToDictionary(
            x => x.Word,
            x => x.DocumentCount > 0 
                ? Math.Log10((double)totalDocuments / x.DocumentCount) 
                : 0
        );
    }

    public override async Task<int> GetTotalCountAsync()
    {
        using SearchEngineContext context = _contextFactory.CreateDbContext();
        return await context.Set<Term>()
            .AsNoTracking()
            .CountAsync();
    }
}