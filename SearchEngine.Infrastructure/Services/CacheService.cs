using Microsoft.Extensions.Caching.Memory;
using SearchEngine.Core.Interfaces.Services;
using SearchEngine.Infrastructure.Extensions;

namespace SearchEngine.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public T? GetOrCreate<T>(string key, Func<T> factory, TimeSpan? expiration = null)
    {
        return _cache.GetOrSet(key, factory, expiration ?? _defaultExpiration);
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (_cache.TryGetValue(key, out T? value))
            return value;

        value = await factory();
        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(expiration ?? _defaultExpiration);

        _cache.Set(key, value, cacheEntryOptions);
        return value;
    }

    public void Remove(string keyPattern)
    {
        if (_cache is MemoryCache memoryCache)
        {
            IEnumerable<string> cacheKeys = memoryCache.GetKeys<string>()
                .Where(k => k.StartsWith(keyPattern));
            
            foreach (string key in cacheKeys)
            {
                _cache.Remove(key);
            }
        }
    }
}