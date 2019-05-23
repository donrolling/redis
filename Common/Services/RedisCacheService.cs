using System;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Common.Services
{
    public class RedisCacheService : IAppCacheService
    {
        private IDatabase _cache;
        private string _databaseConnectionUrl;
        private ConnectionMultiplexer _redis;

        public RedisCacheService(string databaseConnectionUrl = "localhost")
        {
            _databaseConnectionUrl = databaseConnectionUrl;
            setup();
        }

        public string Get(string key)
        {
            return get(key);
        }

        public string Get(string key, string defaultValue)
        {
            var x = get(key);
            if (x.IsNullOrEmpty)
            {
                return defaultValue;
            }
            return x;
        }

        public T Get<T>(string key)
        {
            var x = Get(key);
            if (string.IsNullOrEmpty(x))
            {
                return JsonConvert.DeserializeObject<T>(x);
            }
            return default(T);
        }

        public T Get<T>(string key, T defaultValue)
        {
            var x = Get<T>(key);
            if (x != null)
            {
                return (T)x;
            }
            return defaultValue;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var x = await getAsync(key);
            if (!x.IsNullOrEmpty)
            {
                return JsonConvert.DeserializeObject<T>(x);
            }
            return default(T);
        }

        public async Task<T> GetAsync<T>(string key, T defaultValue)
        {
            var x = await GetAsync<T>(key);
            if (x != null)
            {
                return x;
            }
            return defaultValue;
        }

        public async Task<string> GetAysnc(string key)
        {
            return await getAsync(key);
        }

        public T GetSet<T>(string key, Func<T> func, int expireTimeInMinutes = 20)
        {
            var x = Get<T>(key);
            if (x == null)
            {
                var y = func.Invoke();
                set<T>(key, y, expireTimeInMinutes);
                return y;
            }
            return x;
        }

        public async Task<T> GetSetAsync<T>(string key, Func<Task<T>> func, int expireTimeInMinutes = 20)
        {
            var x = await GetAsync<T>(key);
            if (x == null)
            {
                var y = await func.Invoke();
                await setAsync<T>(key, y, expireTimeInMinutes);
                return y;
            }
            return x;
        }

        public void Remove(string key)
        {
            remove(key);
        }

        public async Task<bool> RemoveAsync(string key)
        {
            return await removeAsync(key);
        }

        public void Set(string key, string value)
        {
            set(key, value);
        }

        public void Set<T>(string key, T entity, int expireTimeInMinutes = 20)
        {
            set<T>(key, entity, expireTimeInMinutes);
        }

        public async Task<bool> SetAsync<T>(string key, T entity, int expireTimeInMinutes = 20)
        {
            return await setAsync<T>(key, entity, expireTimeInMinutes);
        }

        private RedisValue get(string key)
        {
            if (key.IsNullOrEmpty())
            {
                throw new Exception("Key cannot be empty.");
            }
            return _cache.StringGet(key);
        }

        private async Task<RedisValue> getAsync(string key)
        {
            if (key.IsNullOrEmpty())
            {
                throw new Exception("Key cannot be empty.");
            }
            return await getAsync(key);
        }

        private void remove(string key)
        {
            _cache.KeyDelete(key);
        }

        private async Task<bool> removeAsync(string key)
        {
            return await _cache.KeyDeleteAsync(key);
        }

        private void set(string key, string value)
        {
            _cache.StringSet(key, value);
        }

        private void set<T>(string key, T value, int expireTimeInMinutes)
        {
            if (key.IsNullOrEmpty())
            {
                throw new Exception("Key cannot be empty.");
            }
            if (value == null)
            {
                return;
            }
            var x = JsonConvert.SerializeObject(value);
            _cache.StringSet(key, x, TimeSpan.FromMinutes(expireTimeInMinutes));
        }

        private async Task<bool> setAsync<T>(string key, T value, int expireTimeInMinutes)
        {
            if (key.IsNullOrEmpty())
            {
                throw new Exception("Key cannot be empty.");
            }
            if (value == null)
            {
                return false;
            }
            var x = JsonConvert.SerializeObject(value);
            await _cache.StringSetAsync(key, x, TimeSpan.FromMinutes(expireTimeInMinutes));
            return true;
        }

        private void setup()
        {
            _redis = ConnectionMultiplexer.Connect(_databaseConnectionUrl);
            _cache = _redis.GetDatabase();
        }
    }
}