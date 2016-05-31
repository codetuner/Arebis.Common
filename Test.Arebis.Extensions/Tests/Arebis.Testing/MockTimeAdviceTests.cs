using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Runtime.Aspects;
using Arebis.Testing;
using System.Threading;

namespace Arebis.Extensions.Tests.Arebis.Testing
{
	[TestClass]
	[Advisable]
	public class MockTimeAdviceTests : ContextBoundObject
	{
		[TestMethod]
		[MockTimeAdvice("2007/10/02 14:32:10")]
		public void FixedTimeTest()
		{
			DateTime expected = new DateTime(2007, 10, 02, 14, 32, 10, DateTimeKind.Local);
			Assert.AreEqual(global::System.Current.DateTime.Now, expected);
			Thread.Sleep(100);
            Assert.AreEqual(expected, global::System.Current.DateTime.Now);
            Assert.AreEqual(expected.ToUniversalTime(), global::System.Current.DateTime.UtcNow);
            Assert.AreEqual(expected.Date, global::System.Current.DateTime.Today);
            Assert.AreEqual(DateTimeKind.Local, global::System.Current.DateTime.Now.Kind);
            Assert.AreEqual(DateTimeKind.Utc, global::System.Current.DateTime.UtcNow.Kind);
		}

		[TestMethod]
		[MockTimeAdvice("2007/10/02 14:32:10", 0.0)]
		public void Accelerated0TimeTest()
		{
			DateTime expected = new DateTime(2007, 10, 02, 14, 32, 10);
            Assert.AreEqual(global::System.Current.DateTime.Now, expected);
			Thread.Sleep(100);
            Assert.AreEqual(global::System.Current.DateTime.Now, expected);
		}

		[TestMethod]
		[MockTimeAdvice("2007/10/02 14:32:10", 60.0)]
		public void Accelerated60TimeTest()
		{
			DateTime initial = new DateTime(2007, 10, 02, 14, 32, 10);
			Thread.Sleep(500);
            Assert.AreEqual(global::System.Current.DateTime.Now, initial);
			Thread.Sleep(500);
            Assert.IsTrue(global::System.Current.DateTime.Now.DiffAbs(initial.AddSeconds(30)).Seconds < 5);
		}

		[TestMethod]
		[MockTimeAdvice("2007/10/02 14:32:10", "00:05:10")]
		public void IncrementedTimeTest()
		{
			DateTime initial = new DateTime(2007, 10, 02, 14, 32, 10);
			Thread.Sleep(100);
            Assert.AreEqual(global::System.Current.DateTime.Now, initial);
            Assert.AreEqual(global::System.Current.DateTime.Now, initial.AddSeconds(1 * 310));
			Thread.Sleep(100);
            Assert.AreEqual(global::System.Current.DateTime.Now, initial.AddSeconds(2 * 310));
            Assert.AreEqual(global::System.Current.DateTime.Now, initial.AddSeconds(3 * 310));
		}
	}
}
