using SearchEngine.Shared.DTOs.Responses;

namespace SearchEngine.Core.Records;

public record DocumentScore(double Score, List<TermOccurrenceResponse> Occurrences);