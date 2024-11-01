using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SearchEngine.Core.Configuration;
using SearchEngine.Core.Interfaces.Services;
using SearchEngine.Infrastructure.Services;

namespace SearchEngine.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GutenbergSettings>(
            configuration.GetSection("GutenbergApi"));
        services.AddHttpClient<IGutenbergService, GutenbergService>();
        services.AddScoped<ILanguageDetectionService, LanguageDetectionService>();
        services.AddScoped<ICacheService, CacheService>();

        return services;
    }
}