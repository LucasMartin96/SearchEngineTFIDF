namespace SearchEngine.Core.Interfaces.Services;

public interface ICacheService
{
    T? GetOrCreate<T>(string key, Func<T> factory, TimeSpan? expiration = null);
    Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    void Remove(string keyPattern);
}