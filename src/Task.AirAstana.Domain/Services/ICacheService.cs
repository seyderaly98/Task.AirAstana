namespace Task.AirAstana.Domain.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    System.Threading.Tasks.Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
    System.Threading.Tasks.Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
}