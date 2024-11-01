using Microsoft.AspNetCore.Mvc;
using SearchEngine.Core.Entities;
using SearchEngine.Core.Interfaces.Services;
using SearchEngine.Shared.DTOs;
using SearchEngine.Shared.DTOs.Base;
using SearchEngine.Shared.DTOs.Requests;
using SearchEngine.Shared.DTOs.Responses;

namespace SearchEngine.API.Controllers;



[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    
    /// <summary>
    /// Searches documents with pagination
    /// </summary>
    /// <param name="request">Search parameters including pagination</param>
    /// <returns>Paginated list of documents matching the search query</returns>
    /// <response code="200">Returns the paginated search results</response>
    /// <response code="400">If the query parameters are invalid</response>
    [HttpGet]
    public async Task<ActionResult<PagedResult<Document>>> Search([FromQuery] SearchRequest request)
    {
        PagedResult<SearchResultResponse> result = await _searchService.SearchAsync(request.Query, request);
        return Ok(result);
    }
}