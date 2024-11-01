using SearchEngine.Shared.DTOs.Base;

namespace SearchEngine.Core.Helpers;

public static class PaginationHelper
{
    public static PagedResult<T> CreateEmptyResult<T>(PaginationParameters parameters)
    {
        return new PagedResult<T>
        {
            Items = Enumerable.Empty<T>().ToList(),
            TotalCount = 0,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            TotalPages = 0
        };
    }
} 