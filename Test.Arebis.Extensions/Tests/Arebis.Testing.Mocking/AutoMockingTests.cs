using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Mocking;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking
{

	[TestClass()]
	public class AutoMockingTests : _MockingTester
	{
		[TestInitialize()]
		public override void TestSetup()
		{
			base.TestSetup();
			Sample.CurrencyServiceFactory.NextInstance = null;
		}

		[TestMethod()]
		public void MockCreatedTest()
		{
			using (RecorderManager.NewRecordingSession("test"))
			{
				Assert.AreEqual(RecorderState.Recording, RecorderManager.Action);
				MockService.AddTypeToMock(typeof(Sample.Account));
				Sample.Account acc = new Sample.Account(Sample.CurrencyUnit.EUR);
				Assert.IsTrue(MockService.IsMock(acc));
			}
		}

		[TestMethod()]
		public void MockNotCreatedTest()
		{
			// When type not added with AddTypeToMock...
			using (RecorderManager.NewRecordingSession("test"))
			{
				Assert.AreEqual(RecorderState.Recording, RecorderManager.Action);
				Sample.Account acc = new Sample.Account(Sample.CurrencyUnit.EUR);
				Assert.IsFalse(MockService.IsMock(acc));
			}
		}

		[TestMethod()]
		public void MessageTest()
		{
			using (RecorderManager.NewRecordingSession("test"))
			{
				MockService.AddTypeToMock(typeof(Sample.Account));
				Sample.Account acc = new Sample.Account(Sample.CurrencyUnit.EUR);
				acc.Deposit(100m);
				decimal v = acc.Balance;
			}
			Assert.IsTrue(CurrentRecorder[0].IsConstructorCall);
			Assert.IsFalse(CurrentRecorder[1].IsConstructorCall);
			Assert.AreEqual("Deposit", CurrentRecorder[1].Method.Name);
			Assert.AreEqual(1, CurrentRecorder[1].InArgs.Length);
			Assert.AreEqual(100m, CurrentRecorder[1].InArgs[0]);
			Assert.IsNull(CurrentRecorder[1].ReturnValue);
			Assert.IsFalse(CurrentRecorder[2].IsConstructorCall);
			Assert.AreEqual("get_Balance", CurrentRecorder[2].Method.Name);
			Assert.AreEqual(0, CurrentRecorder[2].InArgs.Length);
			Assert.AreEqual(100m, CurrentRecorder[2].ReturnValue);
		}

		[TestMethod()]
		public void MessageCompletedTest()
		{
			using (RecorderManager.NewRecordingSession("test"))
			{
				MockService.AddTypeToMock(typeof(Sample.Account));
				Sample.Account acc = new Sample.Account(Sample.CurrencyUnit.EUR);
				acc.Deposit(100m);
				decimal v = acc.Balance;
			}
			Assert.IsTrue(CurrentRecorder[0].IsCompleted);
			Assert.IsTrue(CurrentRecorder[1].IsCompleted);
			Assert.IsTrue(CurrentRecorder[2].IsCompleted);
		}


		[TestMethod()]
		public void MockOutArgsTest()
		{
			using (RecorderManager.NewRecordingSession("test"))
			{
				Assert.AreEqual(RecorderState.Recording, RecorderManager.Action);
				// Scenario:
				Sample.FooBar fb = new Sample.FooBar();
				int a = 1, b = 2, c, d = 4;
				object result = fb.SomeMethodWithInsAndOuts(a, ref b, out c, d);
				// Check results:
				Assert.IsNotNull(result);
				Assert.AreEqual(3, b);
				Assert.AreEqual(7, c);
				// Check mocking records:
				Assert.IsTrue(CurrentRecorder[0].IsConstructorCall);
				Assert.IsFalse(CurrentRecorder[1].IsConstructorCall);
				Assert.IsTrue(CurrentRecorder[1].InArgs.Length == 3);
				Assert.AreEqual(1, CurrentRecorder[1].InArgs[0]);
				Assert.AreEqual(2, CurrentRecorder[1].InArgs[1]);
				Assert.AreEqual(4, CurrentRecorder[1].InArgs[2]);
				Assert.IsTrue(CurrentRecorder[1].OutArgs.Length == 2);
				Assert.AreEqual(3, CurrentRecorder[1].OutArgs[0]);
				Assert.AreEqual(7, CurrentRecorder[1].OutArgs[1]);
				Assert.AreEqual(result, CurrentRecorder[1].ReturnValue);
				Assert.AreEqual(1, CurrentRecorder[1].Args[0]);
				Assert.AreEqual(3, CurrentRecorder[1].Args[1]);
				Assert.AreEqual(7, CurrentRecorder[1].Args[2]);
				Assert.AreEqual(4, CurrentRecorder[1].Args[3]);
			}
		}

		[TestMethod()]
		public void RecordAndPlaybackTest()
		{
			decimal balanceToBe;
			Assert.AreEqual(RecorderState.None, RecorderManager.Action);
			using (RecorderManager.NewRecordingSession("test"))
			{
				Assert.AreEqual(RecorderState.Recording, RecorderManager.Action);
				MockService.AddTypeToMock(typeof(Sample.Account));
				MockService.AddTypeToMock(typeof(Sample.CustomizableCurrencyService));
				Sample.Account acc = new Sample.Account(Sample.CurrencyUnit.EUR);
				Assert.IsTrue(MockService.IsMock(acc));
				acc.Deposit(100m);
				acc.SwitchCurrency(Sample.CurrencyUnit.USD);
				acc.Withdraw(50m);
				balanceToBe = acc.Balance;
			}
			Assert.AreEqual(RecorderState.None, RecorderManager.Action);
			Assert.AreEqual(5, CurrentRecorder.Count);
			Assert.IsTrue(CurrentRecorder[0].IsConstructorCall);
			Assert.AreEqual("Deposit", CurrentRecorder[1].Method.Name);
			Assert.AreEqual("SwitchCurrency", CurrentRecorder[2].Method.Name);
			Assert.AreEqual("Withdraw", CurrentRecorder[3].Method.Name);
			Assert.AreEqual("get_Balance", CurrentRecorder[4].Method.Name);
			Assert.AreEqual(balanceToBe, CurrentRecorder[4].ReturnValue);
			using (RecorderManager.NewPlayBackSession("test", false))
			{
				Assert.AreEqual(RecorderState.PlayBack, RecorderManager.Action);
				MockService.AddTypeToMock(typeof(Sample.Account));
				MockService.AddTypeToMock(typeof(Sample.CustomizableCurrencyService));
				Sample.Account acc = new Sample.Account(Sample.CurrencyUnit.EUR);
				Assert.IsTrue(MockService.IsMock(acc));
				acc.Deposit(100m);
				acc.SwitchCurrency(Sample.CurrencyUnit.USD);
				acc.Withdraw(50m);
				Assert.AreEqual(balanceToBe, acc.Balance);
				RecorderManager.ValidatePlayBack();
			}
			Assert.AreEqual(RecorderState.None, RecorderManager.Action);
			Assert.IsTrue(CurrentRecorder.Validated);
		}

		[TestMethod()]
		public void PropertyMock()
		{
			using (RecorderManager.NewRecordingSession("test"))
			{
				MockService.AddTypeToMock(typeof(Sample.Account));
				Sample.Account acc = new Sample.Account(Sample.CurrencyUnit.EUR);
				Assert.IsTrue(MockService.IsMock(acc));
				acc.Deposit(100m);
				acc.Withdraw(50m);
				Assert.IsTrue(acc.IsBalancePositive);
			}
			using (RecorderManager.NewPlayBackSession("test", true))
			{
				MockService.AddTypeToMock(typeof(Sample.Account));
				Sample.Account acc = new Sample.Account(Sample.CurrencyUnit.EUR);
				Assert.IsTrue(MockService.IsMock(acc));
				acc.Deposit(100m);
				acc.Withdraw(50m);
				Assert.IsTrue(acc.IsBalancePositive);
			}
		}

		[TestMethod()]
		public void PlainObjectMockingTest()
		{
			// Running in no mocking state:
			PlainObjectMockingTest_Scenario(false, true);

			// Running recording:
			using (RecorderManager.NewRecordingSession("test"))
			{
				MockService.AddTypeToMock(typeof(Sample.PlainCurrencyService));
				PlainObjectMockingTest_Scenario(true, true);
			}
			// Running playback:
			using (RecorderManager.NewPlayBackSession("test", true))
			{
				MockService.AddTypeToMock(typeof(Sample.PlainCurrencyService));
				PlainObjectMockingTest_Scenario(true, false);
			}
			// Running playback without mocking the type:
			using (RecorderManager.NewRecordingSession("test")) { /* build empty scenario */ }
			using (RecorderManager.NewPlayBackSession("test", true))
			{
				PlainObjectMockingTest_Scenario(false, true);
			}
		}

		private void PlainObjectMockingTest_Scenario(bool mocking, bool plainInstanceCreation)
		{

			// Create instance of a plain object (not a MarshalByRef) for mocking:
			Sample.PlainCurrencyService.WatchInstanceCreation();
			Sample.ICurrencyService serv = (Sample.ICurrencyService)MockingTools.Construct("cs", typeof(Sample.PlainCurrencyService), typeof(Sample.ICurrencyService));

			decimal d1, d2, r1, r2;
			d1 = serv.ConvertAmount(100m, Sample.CurrencyUnit.GBP, Sample.CurrencyUnit.USD);
			d2 = serv.ConvertAmount(d1, Sample.CurrencyUnit.USD, Sample.CurrencyUnit.GBP);

			r1 = serv.GetRate(Sample.CurrencyUnit.USD);
			r2 = serv.GetRate(Sample.CurrencyUnit.GBP);

			// Check mocking:
			Assert.AreEqual(mocking, MockService.IsMock(serv), "Mock should have been created.");
			Assert.AreEqual(plainInstanceCreation, Sample.PlainCurrencyService.IsInstanceCreated);

			// Check call results:
			Assert.AreNotEqual(100m, d1);
			Assert.AreEqual(100m, d2);
			Assert.AreEqual(1.2m, r1);
			Assert.AreEqual(0.7m, r2);
		}
	}
}
