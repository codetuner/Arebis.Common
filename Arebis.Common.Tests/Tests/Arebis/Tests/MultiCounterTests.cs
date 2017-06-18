using Arebis.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Tests
{
    [TestClass]
    public class MultiCounterTests
    {
        [TestMethod]
        public void MultiCounterIncrementScenarioTest()
        {
            var mc = new MultiCounter(10, 10);

            AssertEquals(mc, 0, 0);

            for (int i = 0; i <= 24; i++)
            {
                mc.Increment(1);
            }
            mc.Increment();
            AssertEquals(mc, 6, 2);

            mc.Increment(45);

            AssertEquals(mc, 1, 7);

            mc.Decrement(-7);

            AssertEquals(mc, 8, 7);

            mc.Increment(12);
            mc.Increment(33);
            mc.Increment(329);

            AssertEquals(mc, 2, 5);
        }

        [TestMethod]
        public void MultiCounterDecrementScenarioTest()
        {
            var mc = new MultiCounter(10, 10);

            AssertEquals(mc, 0, 0);

            for (int i = 0; i <= 24; i++)
            {
                mc.Decrement(1);
            }
            mc.Decrement();
            AssertEquals(mc, 4, 7);

            mc.Decrement(45);

            AssertEquals(mc, 9, 2);

            mc.Increment(-7);

            AssertEquals(mc, 2, 2);

            mc.Decrement(12);
            mc.Decrement(33);
            mc.Decrement(329);

            AssertEquals(mc, 8, 4);
        }

        private void AssertEquals(MultiCounter mc, params int[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(mc[i], values[i]);
            }
        }
    }
}
