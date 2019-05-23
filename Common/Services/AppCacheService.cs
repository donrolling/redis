using System;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Common.Services
{
    public class AppCacheService : IAppCacheService
    {
        private const int _defaultExpirationTimeInMinutes = 20;

        public IMemoryCache Cache { get; private set; }

        public AppCacheService(IMemoryCache cache)
        {
            this.Cache = cache;
        }

        public T Get<T>(string key)
        {
            object obj = this.Cache.Get<T>(key);
            if (obj != null)
            {
                return (T)obj;
            }
            return default(T);
        }

        public T Get<T>(string key, T defaultValue)
        {
            object obj = this.Cache.Get<T>(key);
            if (obj != null)
            {
                return (T)obj;
            }
            return defaultValue;
        }

        public string Get(string key)
        {
            return this.Cache.Get<string>(key);
        }

        public string Get(string key, string defaultValue)
        {
            var x = this.Cache.Get<string>(key);
            if (x.IsNullOrEmpty())
            {
                return defaultValue;
            }
            return x;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await new Task<T>(() => this.Get<T>(key));
        }

        public async Task<T> GetAsync<T>(string key, T defaultValue)
        {
            return await new Task<T>(() => this.Get<T>(key, defaultValue));
        }

        public T GetSet<T>(string key, Func<T> func, TimeSpan? expiration = null)
        {
            var item = this.Get<T>(key);
            if (item == null)
            {
                item = func.Invoke();
                if (item == null)
                {
                    return item;
                }

                Set<T>(key, item, expiration);
            }
            return item;
        }

        public async Task<T> GetSetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiration = null)
        {
            var item = this.Get<T>(key);
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
            await SetAsync<T>(key, item, expiration);
            return item;
        }

        public void Remove(string key)
        {
            this.Cache.Remove(key);
        }

        public async Task<bool> RemoveAsync(string key)
        {
            await Task.Run(() => this.Remove(key));
            return true;
        }

        public void Set<T>(string key, T entity, TimeSpan? expiration = null)
        {
            var expireTime = resolveExpiration(expiration);
            this.Cache.Set<T>(key, entity, expireTime);
        }

        public async Task<bool> SetAsync<T>(string key, T entity, TimeSpan? expiration = null)
        {
            await Task.Run(() =>
            {
                this.Cache.Set<T>(key, entity, resolveExpiration(expiration));
            });
            return true;
        }

        private DateTime resolveExpiration(TimeSpan? expiration)
        {
            var expire = (expiration == null || !expiration.HasValue)
                            ? TimeSpan.FromMinutes(_defaultExpirationTimeInMinutes)
                            : expiration.Value;
            return DateTime.UtcNow.Add(expire);
        }
    }
}