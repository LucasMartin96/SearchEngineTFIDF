namespace SearchEngine.Core.Interfaces.Repositories;

using Entities;

public interface ITermOccurrenceRepository : IBaseRepository<TermOccurrence>
{
    Task<Dictionary<(Guid TermId, Guid DocumentId), double>> GetTermFrequenciesAsync(
        List<Guid> termIds, 
        Guid documentId);
}