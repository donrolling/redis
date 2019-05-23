using Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Tests
{
    [TestClass]
    public class RedisTest : TestBase
    {
        public RedisTest()
        {
            this.Init(false);
        }

        [TestMethod]
        public void given_a_redis_cache_provider_then_storing_and_retrieving_items_succeeds()
        {
            var cacheProvider = this.ServiceProvider.GetService<IAppCacheService>();
            Assert.IsNotNull(cacheProvider);
            var key = "testkey";
            var value = "testvalue";
            var x = cacheProvider.GetSet(key, () => { return value; });
            Assert.AreEqual(value, x, "Unexpected value.");
            
            var y = cacheProvider.Get(key);
            Assert.AreEqual(value, y, "Unexpected value.");

            var newValue = "NewTestValue";
            cacheProvider.Set(key, newValue);
            
            var z = cacheProvider.Get(key);
            Assert.AreNotEqual(value, z, "Unexpected value.");
            Assert.AreEqual(newValue, z, "Unexpected value.");
        }
    }
}