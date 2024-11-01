using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SearchEngine.Core.Interfaces.Repositories;
using SearchEngine.Persistence.Context;
using SearchEngine.Persistence.Repositories;

namespace SearchEngine.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<SearchEngineContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ISearchEngineContextFactory, SearchEngineContextFactory>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<ITermRepository, TermRepository>();
        services.AddScoped<ITermOccurrenceRepository, TermOccurrenceRepository>();
        services.AddScoped<IStopWordRepository, StopWordRepository>();

        return services;
    }
}