using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SearchEngine.Core.Interfaces.Repositories;
using SearchEngine.Core.Interfaces.Services;
using SearchEngine.Core.Services;
using SearchEngine.Infrastructure.Services;
using SearchEngine.Persistence.Context;
using SearchEngine.Persistence.Repositories;
using Microsoft.Extensions.Caching.Memory;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<SearchEngineContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache();

builder.Services.AddScoped<ISearchEngineContextFactory, SearchEngineContextFactory>();

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<ITermRepository, TermRepository>();
builder.Services.AddScoped<ITermOccurrenceRepository, TermOccurrenceRepository>();
builder.Services.AddScoped<IStopWordRepository, StopWordRepository>();
builder.Services.AddScoped<ILanguageDetectionService, LanguageDetectionService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IIndexerService, IndexerService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IGutenbergService, GutenbergService>();

builder.Services.AddSingleton<Dictionary<string, HashSet<string>>>(new Dictionary<string, HashSet<string>>());

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddHttpClient();

IHost host = builder.Build();

using IServiceScope scope = host.Services.CreateScope();
IServiceProvider services = scope.ServiceProvider;
IIndexerService indexerService = services.GetRequiredService<IIndexerService>();
ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();

try
{
    await IndexGutenbergBooks(indexerService, logger);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while indexing books");
}

static async Task IndexGutenbergBooks(IIndexerService indexerService, ILogger<Program> logger)
{
    string gutenbergDirectory = "toserver";
    Directory.CreateDirectory(gutenbergDirectory);

    int[] bookIds = new[] { 
        1342, 11, 84, 1661, 98, 2701, 1952, 1080, 74, 16, 345, 1232, 2591, 174, 
        1400, 2814, 158, 1184, 2147, 1260, 768, 2500, 45, 1497, 2600, 219, 1250, 
        2554, 120, 2542, 1322, 215, 244, 135, 161, 2641, 1934, 205, 1064, 514, 
        2852, 1399, 55, 76, 5200, 2097, 1268, 2346, 844
    };

    foreach (int bookId in bookIds)
    {
        try
        {
            string bookPath = Path.Combine(gutenbergDirectory, $"{bookId}.txt");
            string gutenbergUrl = $"https://www.gutenberg.org/cache/epub/{bookId}/pg{bookId}.txt";

            string? content;
            if (!File.Exists(bookPath))
            {
                using HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(5);
                content = await client.GetStringAsync(gutenbergUrl);
                await File.WriteAllTextAsync(bookPath, content);
                logger.LogInformation("Downloaded book {BookId}", bookId);
            }
            
            content = await File.ReadAllTextAsync(bookPath);
            
            string title = ExtractTitle(content) ?? $"Book {bookId}";
            logger.LogInformation("Title Value is {Title}", title);
            
            content = CleanGutenbergText(content);

            await indexerService.IndexDocumentAsync(title, gutenbergUrl, content);
            logger.LogInformation("Indexed book {BookId}: {Title}", bookId, title);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to index book {BookId}", bookId);
        }
    }
}

static string CleanGutenbergText(string content)
{
    string[] startMarkers = new[]
    {
        "*** START OF THIS PROJECT GUTENBERG",
        "***START OF THE PROJECT GUTENBERG",
        "*** START OF THE PROJECT GUTENBERG"
    };

    string[] endMarkers = new[]
    {
        "*** END OF THIS PROJECT GUTENBERG",
        "***END OF THE PROJECT GUTENBERG",
        "*** END OF THE PROJECT GUTENBERG"
    };

    int startIndex = -1;
    int endIndex = content.Length;

    foreach (string marker in startMarkers)
    {
        startIndex = content.IndexOf(marker);
        if (startIndex != -1)
        {
            startIndex = content.IndexOf("\r\n", startIndex) + 2;
            break;
        }
    }

    foreach (string marker in endMarkers)
    {
        endIndex = content.IndexOf(marker);
        if (endIndex != -1) break;
    }

    if (startIndex == -1) startIndex = 0;
    if (endIndex == -1) endIndex = content.Length;

    return content[startIndex..endIndex].Trim();
}

static string? ExtractTitle(string content)
{
    int titleLineMatch = content.IndexOf("Title:", StringComparison.OrdinalIgnoreCase);
    if (titleLineMatch != -1)
    {
        int titleStart = titleLineMatch + "Title:".Length;
        int titleEnd = content.IndexOf('\n', titleStart);
        if (titleEnd != -1)
        {
            return content.Substring(titleStart, titleEnd - titleStart).Trim();
        }
    }
    
    string startMarker = "*** START OF THE PROJECT GUTENBERG EBOOK ";
    int markerIndex = content.IndexOf(startMarker, StringComparison.OrdinalIgnoreCase);
    if (markerIndex != -1)
    {
        int titleStart = markerIndex + startMarker.Length;
        int titleEnd = content.IndexOf(" ***", titleStart);
        if (titleEnd != -1)
        {
            return content.Substring(titleStart, titleEnd - titleStart).Trim();
        }
    }
    
    return null;
}