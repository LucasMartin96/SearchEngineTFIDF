namespace SearchEngine.Core.Interfaces.Services;

using Entities;

public interface IIndexerService
{
    Task<Document> IndexDocumentAsync(string title, string path, string content);
}