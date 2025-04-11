using Microsoft.Extensions.Caching.Distributed;

namespace VideoGameApi.Services.Caching
{
    public interface IRedisCacheService
    {
        T? GetData<T>(string key);
        public void SetData<T>(string key, T data);

        void RemoveData(string key);
    }
}
