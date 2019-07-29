using System;
using System.Threading.Tasks;
using Common.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Common.Cache
{
    public class AppCacheService : IAppCacheService
    {
        private const int _defaultExpirationTimeInMinutes = 20;

        public IMemoryCache Cache { get; private set; }

        public AppCacheService(IMemoryCache cache)
        {
            Cache = cache;
        }

        public T Get<T>(string key)
        {
            object obj = Cache.Get<T>(key);
            if (obj != null)
            {
                return (T)obj;
            }
            return default;
        }

        public T Get<T>(string key, T defaultValue)
        {
            object obj = Cache.Get<T>(key);
            if (obj != null)
            {
                return (T)obj;
            }
            return defaultValue;
        }

        public string Get(string key)
        {
            return Cache.Get<string>(key);
        }

        public string Get(string key, string defaultValue)
        {
            var x = Cache.Get<string>(key);
            if (x.IsNullOrEmpty())
            {
                return defaultValue;
            }
            return x;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await new Task<T>(() => Get<T>(key));
        }

        public async Task<T> GetAsync<T>(string key, T defaultValue)
        {
            return await new Task<T>(() => Get(key, defaultValue));
        }

        public T GetSet<T>(string key, Func<T> func, TimeSpan? expiration = null)
        {
            var item = Get<T>(key);
            if (item == null)
            {
                item = func.Invoke();
                if (item == null)
                {
                    return item;
                }

                Set(key, item, expiration);
            }
            return item;
        }

        public async Task<T> GetSetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiration = null)
        {
            var item = Get<T>(key);
            var defaultValue = default(T);
            //todo: not sure if the defaultValue is going to work here.
            if (item != null && !item.Equals(defaultValue))
            {
                return item;
            }
            item = await func.Invoke();
            if (item == null)
            {
                //don't cache null stuff
                return item;
            }
            await SetAsync(key, item, expiration);
            return item;
        }

        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        public async Task<bool> RemoveAsync(string key)
        {
            await Task.Run(() => Remove(key));
            return true;
        }

        public void Set<T>(string key, T entity, TimeSpan? expiration = null)
        {
            var expireTime = resolveExpiration(expiration);
            Cache.Set(key, entity, expireTime);
        }

        public async Task<bool> SetAsync<T>(string key, T entity, TimeSpan? expiration = null)
        {
            await Task.Run(() =>
            {
                Cache.Set(key, entity, resolveExpiration(expiration));
            });
            return true;
        }

        private DateTime resolveExpiration(TimeSpan? expiration)
        {
            var expire = expiration == null || !expiration.HasValue
                            ? TimeSpan.FromMinutes(_defaultExpirationTimeInMinutes)
                            : expiration.Value;
            return DateTime.UtcNow.Add(expire);
        }
    }
}