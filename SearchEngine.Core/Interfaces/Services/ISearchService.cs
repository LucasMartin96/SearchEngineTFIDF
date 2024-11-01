using SearchEngine.Shared.DTOs;
using SearchEngine.Shared.DTOs.Base;
using SearchEngine.Shared.DTOs.Responses;

namespace SearchEngine.Core.Interfaces.Services;

using Entities;

public interface ISearchService
{
    Task<PagedResult<SearchResultResponse>> SearchAsync(string query, PaginationParameters parameters);
    void InvalidateIdfCache();
}