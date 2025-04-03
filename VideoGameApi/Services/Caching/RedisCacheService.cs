using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace VideoGameApi.Services.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public T? GetData<T>(string key)
        {
           var data = _cache?.GetString(key);

            if (data == null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(data);
        }

        public void SetData<T>(string key, T data)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            };

            _cache?.SetString(key, JsonSerializer.Serialize(data));
        }
    }
}
