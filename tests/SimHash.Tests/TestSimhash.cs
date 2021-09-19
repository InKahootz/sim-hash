using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimHashLib;

namespace SimhashTests
{
    [TestClass]
    public class TestSimhash
    {
        //only works with md5 hashing
        [TestMethod]
        public void test_value_by_string()
        {
            var simHash = new SimHash(hashingType: SimHash.HashingType.MD5);
            simHash.GenerateSimhash("aaa bbb test test testing.happy time = -).");
            ulong expected = 5683413558821905382;
            Assert.AreEqual(expected, simHash.value);
        }

        //Exact tests from https://github.com/liangsun/simhash
        [TestMethod]
        public void test_value()
        {
            List<string> features = new List<string>() { "aaa", "bbb" };
            var simHash = new SimHash(hashingType: SimHash.HashingType.MD5);
            simHash.GenerateSimhash(features);
            ulong expected = 8637903533912358349;
            Assert.AreEqual(expected, simHash.value);
        }

        [TestMethod]
        public void test_value_control()
        {
            List<string> features = new List<string>() { "aaa" };
            var simHash = new SimHash(hashingType: SimHash.HashingType.MD5);
            simHash.GenerateSimhash(features);
            ulong expected = 7483809945577191432;
            Assert.AreEqual(expected, simHash.value);
        }

        [TestMethod]
        public void test_distance()
        {
            var sh = new SimHash();
            sh.GenerateSimhash("How are you? I AM fine. Thanks. And you?");
            var sh2 = new SimHash();
            sh2.GenerateSimhash("How old are you? :-) i am fine. Thanks. And you?");
            int distA = sh.Distance(sh2);
            Assert.IsTrue(distA > 0);

            var sh3 = new SimHash(sh2);
            int distB = sh2.Distance(sh3);
            Assert.AreEqual(0, distB);

            var sh4 = new SimHash();
            sh4.GenerateSimhash("1");
            Assert.AreNotEqual(0, sh4.Distance(sh3));
        }
        [TestMethod]
        public void test_chinese()
        {
            var sh = new SimHash();
            sh.GenerateSimhash("你好　世界！　　呼噜。");
            var sh2 = new SimHash();
            sh2.GenerateSimhash("你好，世界呼噜");
            Assert.AreEqual(sh.Distance(sh2), 0);

            var sh4 = new SimHash();
            sh4.GenerateSimhash("How are you? I Am fine. ablar ablar xyz blar blar blar blar blar blar blar Thanks.");
            var sh5 = new SimHash();
            sh5.GenerateSimhash("How are you i am fine.ablar ablar xyz blar blar blar blar blar blar blar than");
            var sh6 = new SimHash();
            sh6.GenerateSimhash("How are you i am fine.ablar ablar xyz blar blar blar blar blar blar blar thank");

            Assert.IsTrue(sh4.Distance(sh6) < 3);
            Assert.IsTrue(sh5.Distance(sh6) < 3);
        }

        [TestMethod]
        public void test_short()
        {
            List<SimHash> shs = new List<SimHash>();
            List<string> ss = new List<string>() { "aa", "aaa", "aaaa", "aaaab", "aaaaabb", "aaaaabbb" };
            foreach (string s in ss)
            {
                var simHash = new SimHash();
                simHash.GenerateSimhash(s);
                shs.Add(simHash);
            }

            foreach (SimHash sh1 in shs)
            {
                foreach (SimHash sh2 in shs)
                {
                    if (sh1 != sh2)
                    {
                        Assert.AreNotEqual(sh1, sh2);
                    }
                }
            }
        }
    }
}
