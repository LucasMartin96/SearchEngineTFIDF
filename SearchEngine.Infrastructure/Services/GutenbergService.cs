using Microsoft.Extensions.Options;
using SearchEngine.Core.Configuration;
using SearchEngine.Core.Interfaces.Services;
using SearchEngine.Shared.DTOs;

namespace SearchEngine.Infrastructure.Services;

public class GutenbergService : IGutenbergService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public GutenbergService(
        HttpClient httpClient, 
        IOptions<GutenbergSettings> settings)
    {
        _httpClient = httpClient;
        _baseUrl = settings.Value.BaseUrl;
    }

    public async Task<GutenbergBookDto?> GetBookAsync(int bookId)
    {
        try
        {
            string url = $"{_baseUrl}/{bookId}/pg{bookId}.txt";
            string content = await _httpClient.GetStringAsync(url);
            
            string[] lines = content.Split('\n');
            string title = lines.FirstOrDefault(l => l.Contains("Title:", StringComparison.OrdinalIgnoreCase))?
                .Replace("Title:", "", StringComparison.OrdinalIgnoreCase)
                .Trim() ?? $"Book {bookId}";

            return new GutenbergBookDto
            {
                Id = bookId,
                Title = title,
                Content = content,
                Url = url,
            };
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
} 