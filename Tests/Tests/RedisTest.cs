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
            var services = this.PreInit(false);
            this.Init(services);
        }

        [TestMethod]
        public void DoStuff()
        {
        }
    }
}