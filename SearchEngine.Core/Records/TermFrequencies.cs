namespace SearchEngine.Core.Records;

public record TermFrequencies(Dictionary<(Guid TermId, Guid DocumentId), double> Frequencies, Dictionary<string, double> Idfs);
