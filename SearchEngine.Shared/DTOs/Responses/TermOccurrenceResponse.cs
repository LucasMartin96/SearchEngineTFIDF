namespace SearchEngine.Shared.DTOs.Responses;

public class TermOccurrenceResponse
{
    public Guid TermId { get; set; }
    public string Value { get; set; } = string.Empty;
    public int Frequency { get; set; }
}