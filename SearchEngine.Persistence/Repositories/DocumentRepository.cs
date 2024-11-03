namespace SearchEngine.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Core.Entities;
using SearchEngine.Core.Interfaces.Repositories;
using Context;

public class DocumentRepository : BaseRepository<Document>, IDocumentRepository
{
    public DocumentRepository(SearchEngineContext context) : base(context)
    {
    }
    
    // TODO: Future Bottleneck here i think, need to analise
    // TODO: Just bring it without tracking changes
    public async Task<List<Document>> SearchAsync(string query)
    {
        string[] queryTerms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string normalizedQuery = query.ToLower();
        
        List<Document> titleResults = await Entities
            .Where(x => x.Title.ToLower().Contains(normalizedQuery))
            .ToListAsync();
        
        List<Document> termResults = await Entities
            .Where(d => d.TermOccurrences
                .Join(Context.Terms.Where(t => queryTerms.Contains(t.Word.ToLower())),
                    to => to.TermId,
                    t => t.Id,
                    (to, t) => t.Id)
                .Any())
            .ToListAsync();
        
        return titleResults.Union(termResults).ToList();
    }

    public async Task<(List<Document> Documents, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        int totalCount = await Entities.CountAsync();
        
        var documents = await Entities
            .OrderByDescending(d => d.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (documents, totalCount);
    }
}