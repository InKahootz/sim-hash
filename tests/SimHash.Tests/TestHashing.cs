using System.Numerics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimHashLib;

namespace SimhashTests
{
    [TestClass]
    public class TestHashing
    {
        [TestMethod]
        public void test_gethashcode_specialhashing_to64bit()
        {
            string eval = "aaa";
            var simHash = new SimHash();
            ulong fromDb = simHash.ComputeJenkinsHash(eval);
            Assert.AreEqual(18323053351575752945, fromDb);
        }

        [TestMethod]
        public void test_hashtostringvalue()
        {
            var simHash = new SimHash();
            string val = simHash.ComputeMD5HashAsHex("aaa");
            Assert.AreEqual(val, "47bce5c74f589f4867dbd57e9ca9f808");
        }

        [TestMethod]
        public void test_hashstringtobigint()
        {
            var simHash = new SimHash();
            BigInteger actualBiggie = simHash.HexToBigIntegter("47bce5c74f589f4867dbd57e9ca9f808");
            string expectedBiggie = "95355999972893604581396806948474189832";
            Assert.AreEqual(expectedBiggie, actualBiggie.ToString());
        }
    }
}
