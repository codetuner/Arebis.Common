using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Mocking;
using Arebis.Testing.Mocking.Manual;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking
{
	[TestClass()]
	public class ManualMockingTests : _MockingTester
	{

		[TestMethod()]
		public void ManualMockingCompatibilityTest()
		{
			ManualMockSession sess = new ManualMockSession();
			Sample.ICurrencyService cv = (Sample.ICurrencyService)sess.Mock(typeof(Sample.ICurrencyService), "cs").Mock;
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidCastException))]
		public void ManualMockingIncompatibilityTest()
		{
			ManualMockSession sess = new ManualMockSession();
			ICustomFormatter cf = (ICustomFormatter)sess.Mock(typeof(Sample.ICurrencyService), "cs").Mock;
		}


		[TestMethod()]
		public void ManualMockingOutArgTest()
		{
			ManualMockSession sess = new ManualMockSession();
			Sample.FooBar fb = (Sample.FooBar)sess.Mock(typeof(Sample.FooBar), "fb")
				.ExpectAndReturn("SomeMethodWithInsAndOuts", "OK", new object[] { 10, 11 })
				.Mock;
			int a = 1, b = 2, c, d = 4;
			object result = fb.SomeMethodWithInsAndOuts(a, ref b, out c, d);
			Assert.AreEqual("OK", result);
			Assert.AreEqual(b, 10);
			Assert.AreEqual(c, 11);
		}


		[TestMethod()]
		public void ManualMockingValidate00Test()
		{
			ManualMockSession sess = new ManualMockSession();
			Sample.ICurrencyService cv = (Sample.ICurrencyService)sess.Mock(typeof(Sample.ICurrencyService), "cs")
				.ExpectAndReturn("ConvertAmount", 50m)
				.Mock;
			cv.ConvertAmount(100m, Sample.CurrencyUnit.EUR, Sample.CurrencyUnit.USD);
			sess.ValidateSession();
		}

		[TestMethod()]
		[ExpectedException(typeof(ReplayMockException))]
		public void ManualMockingValidate01Test()
		{
			ManualMockSession sess = new ManualMockSession();
			Sample.ICurrencyService cv = (Sample.ICurrencyService)sess.Mock(typeof(Sample.ICurrencyService), "cs")
				.ExpectAndReturn("ConvertAmount", 50m)
				.Mock;
			sess.ValidateSession();
		}

		[TestMethod()]
		[ExpectedException(typeof(ReplayMockException))]
		public void ManualMockingValidate02Test()
		{
			ManualMockSession sess = new ManualMockSession();
			Sample.ICurrencyService cv = (Sample.ICurrencyService)sess.Mock(typeof(Sample.ICurrencyService), "cs")
				.Mock;
			cv.ConvertAmount(100m, Sample.CurrencyUnit.EUR, Sample.CurrencyUnit.USD);
		}

		[TestMethod()]
		public void ManualMockingValidate03Test()
		{
			ManualMockSession sess = new ManualMockSession();
			Sample.ICurrencyService cv = (Sample.ICurrencyService)sess.Mock(typeof(Sample.ICurrencyService), "cs")
				.ExpectAndReturn("GetRate", 1.2m)
				.Mock;
			Assert.AreEqual(1.2m, cv.GetRate(Sample.CurrencyUnit.GBP));
			sess.ValidateSession();
		}

		[TestMethod()]
		[ExpectedException(typeof(ReplayMockException))]
		public void ManualMockingValidate04Test()
		{
			ManualMockSession sess = new ManualMockSession();
			Sample.ICurrencyService cv = (Sample.ICurrencyService)sess.Mock(typeof(Sample.ICurrencyService), "cs")
				.ExpectAndReturn("ConvertAmount", 50m)
				.Mock;
			cv.GetRate(Sample.CurrencyUnit.GBP);
			sess.ValidateSession();
		}


		[TestMethod()]
		public void ManualMockingValidate05Test()
		{
			ManualMockSession sess = new ManualMockSession();
			Sample.ICurrencyService cv = (Sample.ICurrencyService)sess.Mock(typeof(Sample.ICurrencyService), "cs")
				.ExpectAndReturn("ConvertAmount", 50m).WithArguments(ExpectedValue.Any, Sample.CurrencyUnit.EUR, Sample.CurrencyUnit.GBP)
				.Mock;
			try
			{
				cv.ConvertAmount(100m, Sample.CurrencyUnit.EUR, Sample.CurrencyUnit.USD);
				Assert.Fail("Should have thrown exception.");
			}
			catch (ReplayMockException ex)
			{
				Assert.IsTrue(ex.Message.IndexOf("\"to\"") >= 0, "Argument 'to' should be marked to have an invalid value.");
			}
		}


		[TestMethod()]
		public void ManualMockingValidate06Test()
		{
			ManualMockSession sess = new ManualMockSession();
			Sample.FooBar fb = (Sample.FooBar)sess.Mock(typeof(Sample.FooBar), "fb")
				.ExpectAndReturn("SomeMethodWithInsAndOuts", null).WithArguments(ExpectedValue.Any, ExpectedValue.Any, null, 4)
				.Mock;
			try
			{
				int a = 1, b = 2, c = 3, d = 0;
				fb.SomeMethodWithInsAndOuts(a, ref b, out c, d);
				Assert.Fail("Should have thrown exception.");
			}
			catch (ReplayMockException ex)
			{
				Assert.IsTrue(ex.Message.IndexOf("\"fourthin\"") >= 0, "Argument 'fourthin' should be marked to have an invalid value.");
			}
		}


		[TestMethod()]
		public void ManualMockingScenarioTest()
		{
			ManualMockSession sess = new ManualMockSession();
			Sample.ICurrencyService cv = (Sample.ICurrencyService)sess.Mock(typeof(Sample.ICurrencyService), "cs")
				.ExpectAndReturn("ConvertAmount", 50m)
				.ExpectAndReturn("ConvertAmount", 80m).WithArguments(50m, ExpectedValue.OfType(typeof(Sample.CurrencyUnit)), ExpectedValue.OfType(typeof(Sample.CurrencyUnit)))
				.ExpectAndReturn("ConvertAmount", 30m).WithArguments(ExpectedValue.Any, ExpectedValue.OfType(typeof(Sample.CurrencyUnit)), Sample.CurrencyUnit.EUR).RepeatTimes(3)
				.Mock;
			Sample.CurrencyServiceFactory.NextInstance = cv;
			Sample.Account acc = new Sample.Account(Sample.CurrencyUnit.EUR);
			acc.Deposit(100m);
			Assert.AreEqual(100m, acc.Balance);
			acc.SwitchCurrency(Sample.CurrencyUnit.GBP);
			Assert.AreEqual(50m, acc.Balance);
			acc.SwitchCurrency(Sample.CurrencyUnit.USD);
			Assert.AreEqual(80m, acc.Balance);
			acc.SwitchCurrency(Sample.CurrencyUnit.EUR);
			Assert.AreEqual(30m, acc.Balance);
			acc.SwitchCurrency(Sample.CurrencyUnit.EUR);
			Assert.AreEqual(30m, acc.Balance);
			try
			{
				acc.SwitchCurrency(Sample.CurrencyUnit.NULL);
				Assert.Fail("Exception expected.");
			}
			catch (ReplayMockException) { }
			sess.ValidateSession();
		}
	}
}
