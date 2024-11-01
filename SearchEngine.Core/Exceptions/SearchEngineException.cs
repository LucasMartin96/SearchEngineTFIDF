namespace SearchEngine.Core.Exceptions;

public class SearchEngineException : Exception
{
    public SearchEngineException(string message) : base(message) { }
    public SearchEngineException(string message, Exception innerException) 
        : base(message, innerException) { }
}




