namespace SearchEngine.Shared.DTOs.Responses;

public class SearchResultResponse
{
    public Guid DocumentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public double Score { get; set; }
    public List<TermOccurrenceResponse> TermOccurrences { get; set; } = new List<TermOccurrenceResponse>();
}