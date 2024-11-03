using SearchEngine.Core.Interfaces.Repositories;
using SearchEngine.Core.Interfaces.Services;
using SearchEngine.Shared.DTOs.Base;
using SearchEngine.Shared.DTOs.Responses;

namespace SearchEngine.Core.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;

    public DocumentService(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<PagedResult<DocumentResponse>> GetDocumentsAsync(PaginationParameters parameters)
    {
        var (documents, totalCount) = await _documentRepository.GetPagedAsync(
            parameters.PageNumber, 
            parameters.PageSize);

        var documentResponses = documents.Select(d => new DocumentResponse
        {
            Id = d.Id,
            Title = d.Title,
            Path = d.Path,
            WordCount = d.WordCount,
            CreatedAt = d.CreatedAt
        }).ToList();

        return new PagedResult<DocumentResponse>
        {
            Items = documentResponses,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize)
        };
    }
} 