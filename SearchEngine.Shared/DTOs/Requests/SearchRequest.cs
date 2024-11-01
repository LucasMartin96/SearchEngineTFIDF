using SearchEngine.Shared.DTOs.Base;

namespace SearchEngine.Shared.DTOs.Requests;

public class SearchRequest : PaginationParameters
{
    public string Query { get; set; } = string.Empty;
}