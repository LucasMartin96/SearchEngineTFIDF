namespace SearchEngine.Core.Entities;

using Base;

public class Term : BaseEntity
{
    public string Word { get; set; } = null!;
    public int DocumentCount { get; set; }
    public virtual ICollection<TermOccurrence> Occurrences { get; set; } = new List<TermOccurrence>();
}