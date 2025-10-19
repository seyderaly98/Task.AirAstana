using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Task.AirAstana.Domain.Services;

namespace Task.AirAstana.Infrastructure.Caching;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;
    private readonly IConnectionMultiplexer _redis;

    public CacheService( IDistributedCache cache, ILogger<CacheService> logger, IConfiguration configuration)
    {
        _cache = cache;
        _logger = logger;
        
        var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        _redis = ConnectionMultiplexer.Connect(redisConnection);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var cachedData = await _cache.GetAsync(key, cancellationToken);

            if (cachedData == null)
            {
                _logger.LogDebug("Кэш промах для ключа: {Key}", key);
                return null;
            }

            var jsonString = Encoding.UTF8.GetString(cachedData);
            var result = JsonSerializer.Deserialize<T>(jsonString);

            _logger.LogDebug("Кэш попадание для ключа: {Key}", key);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при чтении из кэша. Ключ: {Key}", key);
            return null;
        }
    }

    public async System.Threading.Tasks.Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(value);
            var dataBytes = Encoding.UTF8.GetBytes(jsonString);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
            };

            await _cache.SetAsync(key, dataBytes, options, cancellationToken);

            _logger.LogDebug("Данные сохранены в Redis. Ключ: {Key}, TTL: {Expiration}", key, options.AbsoluteExpirationRelativeToNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при записи в Redis. Ключ: {Key}", key);
        }
    }

    public async System.Threading.Tasks.Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _logger.LogDebug("Удалено из Redis. Ключ: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении из Redis. Ключ: {Key}", key);
        }
    }

    public async System.Threading.Tasks.Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            
            // Получить все ключи по паттерну
            var keys = server.Keys(pattern: pattern).ToArray();
            
            if (keys.Length > 0)
            {
                // Удалить все найденные ключи
                await db.KeyDeleteAsync(keys);
                _logger.LogInformation("Удалено {Count} ключей из Redis по паттерну: {Pattern}", keys.Length, pattern);
            }
            else
            {
                _logger.LogDebug("Ключи по паттерну {Pattern} не найдены", pattern);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении из Redis по паттерну: {Pattern}", pattern);
        }
    }
}