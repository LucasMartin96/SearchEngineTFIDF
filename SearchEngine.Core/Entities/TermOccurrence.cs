namespace SearchEngine.Core.Entities;

using Base;

public class TermOccurrence : BaseEntity
{
    public Guid TermId { get; set; }
    public Guid DocumentId { get; set; }
    public int Frequency { get; set; }
    public virtual Term Term { get; set; } = null!;
    public virtual Document Document { get; set; } = null!;
}