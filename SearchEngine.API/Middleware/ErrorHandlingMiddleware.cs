using System.Net;
using System.Text.Json;
using SearchEngine.Core.Exceptions;
using SearchEngine.Shared.DTOs.Responses;

namespace SearchEngine.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpResponse response = context.Response;
        response.ContentType = "application/json";
        
        ErrorResponse errorResponse = new ErrorResponse
        {
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case DocumentNotFoundException ex:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = ex.Message;
                _logger.LogWarning(ex, "Document not found: {Path}", ex.Path);
                break;

            case InvalidQueryException ex:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = ex.Message;
                _logger.LogWarning(ex, "Invalid query: {Query}", ex.Query);
                break;

            case IndexingException ex:
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                errorResponse.Message = ex.Message;
                _logger.LogError(ex, "Indexing error: {Path}", ex.Path);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An unexpected error occurred.";
                _logger.LogError(exception, "Unhandled exception");
                break;
        }

        string result = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(result);
    }
}

