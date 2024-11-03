using SearchEngine.Shared.DTOs.Base;
using SearchEngine.Shared.DTOs.Responses;

namespace SearchEngine.Core.Interfaces.Services;

public interface IDocumentService
{
    Task<PagedResult<DocumentResponse>> GetDocumentsAsync(PaginationParameters parameters);
} 