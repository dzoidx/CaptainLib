using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CaptainLib.Collections;

namespace Collections.Tests
{
    [TestClass]
    public class MemCacheTests
    {
        [TestMethod]
        public void TestNonexistingKeyInvalidate()
        {
            var cache = new MemCache<string, int>();

            Assert.IsFalse(cache.Invalidate("notexists"));

            cache.GetOrCreate("key", () => 0, TimeSpan.FromSeconds(1));
            Assert.IsTrue(cache.Invalidate("key"));
            Assert.IsFalse(cache.Invalidate("key"));
        }
    }
}
