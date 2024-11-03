namespace SearchEngine.Core.Records;

public record Statistics(int TotalDocs, long TotalWords, DateTime? LastIndexed);