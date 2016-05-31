using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arebis.Extensions;

namespace Arebis.Common.Tests.Arebis.Extensions
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void ToEpochTimeTests()
        {
            // Test Epoch root:
            var dt1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(0, dt1.ToEpochTime());

            // Test a random date:
            var dt2 = new DateTime(2016, 3, 19, 16, 43, 59, DateTimeKind.Utc);
            Assert.AreEqual(1458405839, dt2.ToEpochTime());

            // Test a random date after 2038:
            var dt3 = new DateTime(2296, 3, 19, 16, 43, 59, DateTimeKind.Utc);
            Assert.AreEqual(10294361039, dt3.ToEpochTime());
        }

        [TestMethod]
        public void ToExcelTimeTests()
        {
            // Test Excel date root:
            var dt1 = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(1.0, dt1.ToExcelTime(), 0.0001);

            // Test Epoch root:
            var dt2 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(25568.0, dt2.ToExcelTime(), 0.0001);

            // Test a random date:
            var dt3 = new DateTime(2016, 3, 19, 16, 43, 59, DateTimeKind.Utc);
            Assert.AreEqual(42447.6972, dt3.ToExcelTime(), 0.0001);

            // Test a random date in far future:
            var dt4 = new DateTime(2296, 3, 19, 3, 12, 17, DateTimeKind.Utc);
            Assert.AreEqual(144715.1335, dt4.ToExcelTime(), 0.0001);

            // Test a random date close to BC:
            var dt5 = new DateTime(3, 3, 19, 16, 43, 59, DateTimeKind.Utc);
            Assert.AreEqual(-692786.3028, dt5.ToExcelTime(), 0.0001);
        }
    }
}
