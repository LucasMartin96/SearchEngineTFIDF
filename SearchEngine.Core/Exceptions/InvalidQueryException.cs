namespace SearchEngine.Core.Exceptions;

public class InvalidQueryException : SearchEngineException
{
    public string Query { get; }
    public InvalidQueryException(string query) 
        : base($"Invalid search query: {query}")
    {
        Query = query;
    }
}