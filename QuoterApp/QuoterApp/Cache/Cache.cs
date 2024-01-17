using System;
using System.Runtime.Caching;

namespace QuoterApp.Cache
{
    public class Cache : ICache
    {
        private readonly MemoryCache _cache;
        private TimeSpan _defaultExpiration;
        private CacheItemPolicy _cachePolicy;

        public Cache()
        {
            _cache = MemoryCache.Default;
            _defaultExpiration = TimeSpan.FromHours(1);
            _cachePolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.Add(_defaultExpiration)
            };
        }

        public T Get<T>(string key)
        {
            if (_cache.Contains(key))
            {
                return (T)_cache.Get(key);
            }

            return default(T);
        }

        public bool Contains(string key)
        {
            return _cache.Contains(key);
        }

        public void Add<T>(string key, T value)
        {
            _cache.Set(key, value, _cachePolicy);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void SetExpirationTime(TimeSpan expirationTime)
        {
            _cachePolicy.AbsoluteExpiration = DateTimeOffset.Now.Add(expirationTime);
        }
    }
}
