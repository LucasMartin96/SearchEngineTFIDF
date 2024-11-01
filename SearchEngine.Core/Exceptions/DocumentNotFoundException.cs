namespace SearchEngine.Core.Exceptions;

public class DocumentNotFoundException : SearchEngineException
{
    public string Path { get; }
    public DocumentNotFoundException(string path) 
        : base($"Document not found at path: {path}")
    {
        Path = path;
    }
}