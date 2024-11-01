namespace SearchEngine.Core.Exceptions;

public class IndexingException : SearchEngineException
{
    public string Path { get; }
    public IndexingException(string path, string message) 
        : base($"Error indexing document at {path}: {message}")
    {
        Path = path;
    }
}