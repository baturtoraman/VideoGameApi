using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace VideoGameApi.Services.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public T? GetData<T>(string key)
        {
            try
            {
                var data = _cache?.GetString(key);
                if (data == null)
                {
                    _logger.LogWarning("Cache miss for key: {Key}", key);
                    return default;
                }
                return JsonSerializer.Deserialize<T>(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving data from Redis for key: {Key}", key);
                return default;
            }
        }

        public void SetData<T>(string key, T data)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                };

                _cache?.SetString(key, JsonSerializer.Serialize(data), options);
                _logger.LogInformation("Data successfully stored in Redis for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while storing data in Redis for key: {Key}", key);
            }
        }
    }
}
