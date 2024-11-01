using SearchEngine.Core.Entities;
using SearchEngine.Shared.DTOs.Responses;

namespace SearchEngine.Core.Records;

public record ScoredDocument(Document Document, double Score, List<TermOccurrenceResponse> Occurrences);
