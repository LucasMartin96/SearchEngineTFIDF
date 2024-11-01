using System.Reflection;
using Microsoft.Extensions.Caching.Memory;

namespace SearchEngine.Infrastructure.Extensions;

public static class MemoryCacheExtensions
{
    public static T GetOrSet<T>(this IMemoryCache cache, string key, Func<T> factory, TimeSpan expiration)
    {
        if (cache.TryGetValue(key, out T? value))
            return value!;

        value = factory();
        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(expiration);

        cache.Set(key, value, cacheEntryOptions);
        return value;
    }

    public static IEnumerable<string> GetKeys<T>(this MemoryCache memoryCache)
    {
        PropertyInfo? field = typeof(MemoryCache).GetProperty("EntriesCollection", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        dynamic? collection = field?.GetValue(memoryCache) as dynamic;
        dynamic? items = collection?.GetEnumerator();
        
        while (items?.MoveNext() ?? false)
        {
            dynamic? key = items.Current.Key.ToString();
            if (key is not null)
                yield return key;
        }
    }
}