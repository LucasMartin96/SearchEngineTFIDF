namespace SearchEngine.Core.Entities;

using Base;

public class Document : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Path { get; set; } = null!;
    public int WordCount { get; set; }
    public virtual ICollection<TermOccurrence> TermOccurrences { get; set; } = new List<TermOccurrence>();
}