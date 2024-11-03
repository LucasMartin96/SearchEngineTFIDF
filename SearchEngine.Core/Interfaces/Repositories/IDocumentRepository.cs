namespace SearchEngine.Core.Interfaces.Repositories;

using Entities;

public interface IDocumentRepository : IBaseRepository<Document>
{
    Task<List<Document>> SearchAsync(string query);
    Task<(List<Document> Documents, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
}