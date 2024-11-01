namespace SearchEngine.Core.Interfaces.Repositories;

using Entities;

public interface ITermRepository : IBaseRepository<Term>
{
    Task<Term?> GetByWordAsync(string word);
    Task<List<Term>> GetByWordsAsync(List<string> words);
    Task<Dictionary<string, double>> GetInverseDocumentFrequenciesAsync(List<string> words);
}