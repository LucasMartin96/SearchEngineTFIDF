namespace SearchEngine.Shared.DTOs.Responses;

public class StatisticsResponse
{
    public int TotalDocuments { get; set; }
    public int TotalUniqueTerms { get; set; }
    public long TotalWordCount { get; set; }
} 