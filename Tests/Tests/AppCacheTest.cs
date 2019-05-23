using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Models;

namespace Tests.Tests
{
    [TestClass]
    public class AppCacheTest : TestBase
    {
        public AppCacheTest()
        {
            var services = this.PreInit();
            //do config stuff here
            this.Init(services);
        }

        [TestMethod]
        public void DoStuff() {

        }
    }
}
