using Microsoft.AspNetCore.Mvc;
using SearchEngine.Core.Entities;
using SearchEngine.Core.Interfaces.Services;
using SearchEngine.Shared.DTOs;
using SearchEngine.Shared.DTOs.Requests;
using SearchEngine.Shared.DTOs.Responses;

namespace SearchEngine.API.Controllers;

public class IndexController : BaseController
{
    private readonly IIndexerService _indexerService;
    private readonly IGutenbergService _gutenbergService;

    public IndexController(IIndexerService indexerService, IGutenbergService gutenbergService)
    {
        _indexerService = indexerService;
        _gutenbergService = gutenbergService;
    }

    /// <summary>
    /// Indexes a book from Project Gutenberg by its ID
    /// </summary>
    /// <param name="bookId">The Project Gutenberg book ID to index</param>
    /// <returns>Information about the indexed document including its ID and word count</returns>
    /// <response code="200">Returns the indexed document information</response>
    /// <response code="404">If the book was not found in Project Gutenberg</response>
    /// <response code="500">If there was an error processing the book</response>
    [HttpPost("gutenberg/{bookId:int}")]
    public async Task<ActionResult<IndexResponse>> IndexGutenbergBook(int bookId)
    {
        GutenbergBookDto? book = await _gutenbergService.GetBookAsync(bookId);
        
        if (book == null)
        {
            return NotFound($"Book with ID {bookId} not found");
        }

        Document document = await _indexerService.IndexDocumentAsync(
            book.Title,
            book.Url,
            book.Content
        );

        return Ok(new IndexResponse
        {
            DocumentId = document.Id,
            WordCount = document.WordCount
        });
    }
    
    /// <summary>
    /// [Deprecated] Legacy endpoint for indexing documents
    /// </summary>
    /// <param name="request">The document indexing request</param>
    /// <returns>410 Gone status code with deprecation message</returns>
    /// <response code="410">Indicates that this endpoint is no longer available</response>
    [HttpPost]
    [Obsolete("This endpoint is deprecated. Please use /gutenberg instead.")]
    public ActionResult<IndexResponse> Index([FromBody] IndexDocumentRequest request)
    {
        return StatusCode(410, "This endpoint is deprecated. Please use /gutenberg instead.");
    }
}