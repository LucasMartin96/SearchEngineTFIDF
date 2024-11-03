using SearchEngine.Core.Entities;
using SearchEngine.Core.Records;


namespace SearchEngine.Core.Interfaces.Repositories;



public interface IDocumentRepository : IBaseRepository<Document>
{
    Task<List<Document>> SearchAsync(string query);
    Task<(List<Document> Documents, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    Task<Statistics> GetDocumentStatisticsAsync();
}