using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SearchEngine.Core.Interfaces.Services;
using SearchEngine.Core.Services;

namespace SearchEngine.Core.Extensions;

public static class CoreExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IIndexerService, IndexerService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IStatisticsService, StatisticsService>();

        return services;
    }
}