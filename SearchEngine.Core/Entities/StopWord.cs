namespace SearchEngine.Core.Entities;

using Base;

public class StopWord : BaseEntity
{
    public string Word { get; set; } = null!;
    public string Language { get; set; } = null!;
}