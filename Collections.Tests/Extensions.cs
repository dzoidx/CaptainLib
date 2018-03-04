using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CaptainLib.Collections;

namespace Collections.Tests
{
    [TestClass]
    public class Extensions
    {
        [TestMethod]
        public void TestCollectionRandomize()
        {
            var original = Enumerable.Range(0, 10);
            var copy = original.ToList();

            var valuesO = string.Join(",", original);
            var random = original.Randomize();
            var valuesR = string.Join(",", original.Randomize());

            Assert.AreNotEqual(valuesR, valuesO);
            Assert.IsTrue(original.All(_ => random.Contains(_)));

            random = copy.RandomizeMutable();

            Assert.IsTrue(ReferenceEquals(copy, random));
        }
    }
}
