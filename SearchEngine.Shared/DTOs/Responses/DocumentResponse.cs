namespace SearchEngine.Shared.DTOs.Responses;

public class DocumentResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int WordCount { get; set; }
    public DateTime CreatedAt { get; set; }
} 