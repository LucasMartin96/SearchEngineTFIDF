namespace SearchEngine.Core.Interfaces.Repositories;

using Entities;

public interface IDocumentRepository : IBaseRepository<Document>
{
    Task<List<Document>> SearchAsync(string query);
}