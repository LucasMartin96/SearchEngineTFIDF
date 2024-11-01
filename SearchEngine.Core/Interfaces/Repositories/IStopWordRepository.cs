namespace SearchEngine.Core.Interfaces.Repositories;

using Entities;

public interface IStopWordRepository : IBaseRepository<StopWord>
{
    Task<Dictionary<string, HashSet<string>>> GetAllStopWordsByLanguageAsync();
}