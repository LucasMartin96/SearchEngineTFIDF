namespace SearchEngine.Shared.DTOs.Responses;

public class IndexResponse
{
    public Guid DocumentId { get; set; }
    public int WordCount { get; set; }
}