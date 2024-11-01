using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SearchEngine.Persistence.Context;

public interface ISearchEngineContextFactory
{
    SearchEngineContext CreateDbContext();
}

public class SearchEngineContextFactory : ISearchEngineContextFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SearchEngineContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public SearchEngineContext CreateDbContext()
    {
        return _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<SearchEngineContext>();
    }
} 