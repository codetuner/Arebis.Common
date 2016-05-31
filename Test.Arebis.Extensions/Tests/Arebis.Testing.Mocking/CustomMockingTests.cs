using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Mocking;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking
{

	[TestClass()]
	public class CustomMockingTests : _MockingTester
	{
		[TestInitialize()]
		public override void TestSetup()
		{
			base.TestSetup();
			Sample.CurrencyServiceFactory.NextInstance = null;
		}	

		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException), "To test mocking, this instance should never be created.")]
		public void NoMockingTest()
		{
			using (RecorderManager.NewPlayBackSession("test", false))
			{
				Sample.ICurrencyService srv = Sample.CurrencyServiceFactory.NextInstance;
				Assert.IsFalse(MockService.IsMock(srv));
			}
		}

		[TestMethod()]
		public void MockingTest()
		{
			using (RecorderManager.NewPlayBackSession("test", false))
			{
				MockService.AddTypeToMock(typeof(Sample.CustomizableCurrencyService));
				Sample.ICurrencyService srv = Sample.CurrencyServiceFactory.NextInstance;
				Assert.IsTrue(MockService.IsMock(srv));
			}
		}

		[TestMethod()]
		public void CustomMockingScenarioTest()
		{
			using (RecorderManager.NewPlayBackSession("test", false))
			{
				MockService.AddTypeToMock(typeof(Sample.CustomizableCurrencyService));
				Sample.ICurrencyService srv = Sample.CurrencyServiceFactory.NextInstance;
				decimal result = srv.ConvertAmount(100, Sample.CurrencyUnit.NULL, Sample.CurrencyUnit.GBP);
				Assert.AreEqual(100m, result);
			}
		}

		[TestMethod()]
		public void DeepCustomMockingTest()
		{
			using (RecorderManager.NewPlayBackSession("test", false))
			{
				MockService.AddTypeToMock(typeof(Sample.CustomizableCurrencyService));
				Sample.Account acc = new Sample.Account(Sample.CurrencyUnit.EUR);
				acc.Deposit(100m);
				acc.SwitchCurrency(Sample.CurrencyUnit.USD);
				Assert.AreEqual(100m, acc.Balance);
			}
		}
	}
}
