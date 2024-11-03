using Microsoft.AspNetCore.Mvc;
using SearchEngine.Core.Interfaces.Services;
using SearchEngine.Shared.DTOs.Base;
using SearchEngine.Shared.DTOs.Responses;

namespace SearchEngine.API.Controllers;

public class DocumentController : BaseController
{
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    /// <summary>
    /// Gets a paginated list of all indexed documents
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
    /// <returns>Paginated list of documents</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<DocumentResponse>>> GetDocuments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var parameters = new PaginationParameters
        {
            PageNumber = page,
            PageSize = pageSize
        };

        var result = await _documentService.GetDocumentsAsync(parameters);
        return Ok(result);
    }
} 