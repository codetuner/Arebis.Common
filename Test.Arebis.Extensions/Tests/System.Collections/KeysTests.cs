using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace Arebis.Extensions.Tests.System.Collections
{
    [TestClass()]
    public class KeysTests
    {
        [TestMethod()]
        public void Pair01Test()
        {
            Dictionary<Pair<string, int>, String> dict = new Dictionary<Pair<string, int>, string>();
            dict[new Pair<string, int>("alfa", 12)] = "alfa-12";
            dict[new Pair<string, int>("beta", 15)] = "beta-15";
            dict[new Pair<string, int>("gamma", 3)] = "gamma-3";

            Assert.AreEqual("alfa-12", dict[new Pair<string, int>("alfa", 12)]);
            Assert.AreEqual("beta-15", dict[new Pair<string, int>("beta", 15)]);
            Assert.AreEqual("gamma-3", dict[new Pair<string, int>("gamma", 3)]);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Pair02Test()
        {
            Dictionary<Pair<string, int>, String> dict = new Dictionary<Pair<string, int>, string>();
            dict[new Pair<string, int>("alfa", 12)] = "alfa-12";
            dict[new Pair<string, int>("beta", 15)] = "beta-15";
            dict[new Pair<string, int>("gamma", 3)] = "gamma-3";

            string invalidValue = dict[new Pair<string, int>("omega", 3)];
        }
    }
}
