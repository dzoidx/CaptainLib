using Microsoft.VisualStudio.TestTools.UnitTesting;
using CaptainLib.Numbers;
using System.Linq;

namespace Numbers.Tests
{
    [TestClass]
    public class BcdNumberTests
    {
        [TestMethod]
        public void TestCtor()
        {
            var nS = "12389671087263817263897126387126873612873618279";
            var n = new BcdNumber(nS);
            Assert.AreEqual(nS, n.ToString());
            n = new BcdNumber(n);
            Assert.AreEqual(nS, n.ToString());

            nS = "1287398127340958039485.8127398162398";
            n = new BcdNumber(nS);
            Assert.AreEqual(nS, n.ToString());

            nS = "-0.0000008127398162398";
            n = new BcdNumber(nS);
            Assert.AreEqual(nS, n.ToString());
        }

        [TestMethod]
        public void TestShiftLeft()
        {
            var nIntS = "123123123123";
            var nIntS2 = "1231231231230";
            var nS = "0.0000000000" + nIntS;
            var n = new BcdNumber(nS) << (10 + nIntS.Length);
            Assert.AreEqual(nIntS, n.ToString());
            n = new BcdNumber(nS) << (11 + nIntS.Length);
            Assert.AreEqual(nIntS2, n.ToString());
        }

        [TestMethod]
        public void TestMul()
        {
            var n1 = new BcdNumber("22129873182937.734593485097238974");
            var n2 = new BcdNumber("0.1782367");
            var rS = "3944355567545.3181194186252310538371458";
            var r = n1 * n2;
            Assert.AreEqual(rS, r.ToString());
            r = n2 * n1;
            Assert.AreEqual(rS, r.ToString());
        }

        [TestMethod]
        public void TestDiv()
        {
            var piRep = "142857";
            var piS = "3." + string.Join("", Enumerable.Repeat(piRep, BcdNumber.MaxResolution / piRep.Length)) + piRep.Substring(0, BcdNumber.MaxResolution % piRep.Length);
            var n1 = new BcdNumber("22");
            var n2 = new BcdNumber("7");
            var pi = n1 / n2;
            Assert.AreEqual(piS, pi.ToString());
            n1 = new BcdNumber("10000000000000000000");
            n2 = new BcdNumber("0.0000025");
            var rS = "4000000000000000000000000";
            var r = n1 / n2;
            Assert.AreEqual(rS, r.ToString());
            rS = "0.00000000000000000000000025";
            r = n2 / n1;
            Assert.AreEqual(rS, r.ToString());
        }
    }
}
