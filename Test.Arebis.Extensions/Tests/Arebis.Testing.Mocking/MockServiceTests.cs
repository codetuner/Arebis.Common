using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Mocking;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking
{

	[TestClass()]
	public class MockServiceTests : _MockingTester
	{

		[TestMethod()]
		public void AddTypeToMockTest()
		{
			using (RecorderManager.NewPlayBackSession("test", false))
			{
				Assert.IsFalse(MockService.IsTypeToMock(typeof(Sample.CustomizableCurrencyService)));
				MockService.AddTypeToMock(typeof(Sample.CustomizableCurrencyService));
				Assert.IsTrue(MockService.IsTypeToMock(typeof(Sample.CustomizableCurrencyService)));
			}
		}

		[TestMethod()]
		public void AddTypeToMockScopeTest()
		{
			Assert.IsFalse(MockService.IsTypeToMock(typeof(Sample.CustomizableCurrencyService)));
			using (RecorderManager.NewPlayBackSession("test", false))
			{
				Assert.IsFalse(MockService.IsTypeToMock(typeof(Sample.CustomizableCurrencyService)));
				MockService.AddTypeToMock(typeof(Sample.CustomizableCurrencyService));
				Assert.IsTrue(MockService.IsTypeToMock(typeof(Sample.CustomizableCurrencyService)));
			}
			Assert.IsFalse(MockService.IsTypeToMock(typeof(Sample.CustomizableCurrencyService)));
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException), "Types to mock can only be added inside a running playback or recording session.")]
		public void AddTypeToMockOutOfSessionTest()
		{
			MockService.AddTypeToMock(typeof(Sample.CustomizableCurrencyService));
		}
	}
}
