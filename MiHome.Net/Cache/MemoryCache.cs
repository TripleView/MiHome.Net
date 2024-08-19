using Microsoft.Extensions.Caching.Memory;

namespace MiHome.Net.Cache
{
    public class MemoryCache : ICache
    {
        private readonly IMemoryCache memoryCache;
        private readonly ICacheDeserializer cacheDeserializer;
        private readonly ICacheSerializer cacheSerializer;

        public MemoryCache(IMemoryCache memoryCache, ICacheDeserializer cacheDeserializer, ICacheSerializer cacheSerializer)
        {
            this.memoryCache = memoryCache;
            this.cacheDeserializer = cacheDeserializer;
            this.cacheSerializer = cacheSerializer;
        }

        public CacheEntity<T> GetValue<T>(string key)
        {
            var hasValue = memoryCache.TryGetValue(key, out var obj);
            var result = new CacheEntity<T>()
            {
                HasValue = false,
                Data = default
            }; ;
            if (hasValue && obj != null)
            {
                var objPack = obj as ICacheEntry;
                if (objPack == null)
                {
                    return result;
                }
                
                result = cacheDeserializer.DeserializeObject<CacheEntity<T>>(objPack.Value as byte[]);
            }


            return result;
        }

        public async Task<CacheEntity<T>> GetValueAsync<T>(string key)
        {
            return await Task.FromResult( GetValue<T>(key));
        }

        public bool Remove(string key)
        {
            memoryCache.Remove(key);
            return true;
        }

        public async Task<bool> RemoveAsync(string key)
        {
            return await Task.FromResult(Remove(key));
        }

        public bool SetValueWithAbsolute<T>(string key, T value, TimeSpan absoluteExpiration)
        {
            var entity = memoryCache.CreateEntry(key);
            CacheEntity<T> result = new CacheEntity<T>()
            {
                HasValue = true,
                Data = value
            };
            entity.Value = cacheSerializer.SerializeObject(result);
            memoryCache.Set(key, entity, new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = absoluteExpiration });
            return true;
        }

        public async Task<bool> SetValueWithAbsoluteAsync<T>(string key, T value, TimeSpan absoluteExpiration)
        {
            return await Task.FromResult(SetValueWithAbsolute(key, value, absoluteExpiration));
        }

        public bool SetValueWithSliding<T>(string key, T value, TimeSpan slidingExpiration)
        {
            var entity = memoryCache.CreateEntry(key);
            CacheEntity<T> result = new CacheEntity<T>()
            {
                HasValue = true,
                Data = value
            };
            entity.Value = cacheSerializer.SerializeObject(result);
            memoryCache.Set(key, entity, new MemoryCacheEntryOptions() { SlidingExpiration = slidingExpiration });
            return true;
        }

        public async Task<bool> SetValueWithSlidingAsync<T>(string key, T value, TimeSpan slidingExpiration)
        {
            return await Task.FromResult(SetValueWithSliding(key, value, slidingExpiration));
        }
    }
}