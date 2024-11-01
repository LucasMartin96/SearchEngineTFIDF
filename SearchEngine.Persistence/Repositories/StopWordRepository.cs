namespace SearchEngine.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SearchEngine.Core.Entities;
using SearchEngine.Core.Interfaces.Repositories;
using SearchEngine.Persistence.Context;

public class StopWordRepository : BaseRepository<StopWord>, IStopWordRepository
{
    public StopWordRepository(SearchEngineContext context) : base(context)
    {
    }

    // TODO: Just bring it without tracking 
    public async Task<Dictionary<string, HashSet<string>>> GetAllStopWordsByLanguageAsync()
    {
        return await Entities
            .GroupBy(sw => sw.Language)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Select(sw => sw.Word).ToHashSet()
            );
    }
} 