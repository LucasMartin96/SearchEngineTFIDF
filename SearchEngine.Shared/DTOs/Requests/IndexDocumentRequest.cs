namespace SearchEngine.Shared.DTOs.Requests;

// Deprecated
public class IndexDocumentRequest
{
    public string Title { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string Content { get; set; } = null!;
}