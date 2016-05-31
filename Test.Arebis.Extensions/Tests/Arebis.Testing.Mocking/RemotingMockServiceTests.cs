using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Mocking;
using Arebis.Testing.Mocking.Remoting;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking
{

	[TestClass()]
	public class RemotingMockServiceTests : _MockingTester
	{

		[TestMethod()]
		public void GlobalPattern00Test()
		{
			try
			{
				RemotingMockService.AddGlobalMockingUri("*");
				Assert.IsTrue(RemotingMockService.IsUriToMock("http://localhost/service.rem"));
				using (RecorderManager.NewPlayBackSession("test", false))
				{
					Assert.IsTrue(RemotingMockService.IsUriToMock("http://localhost/service.rem"));
				}
				Assert.IsTrue(RemotingMockService.IsUriToMock("http://localhost/service.rem"));
			}
			finally
			{
				RemotingMockService.RemoveGlobalMockingUri("*");
			}
		}

		[TestMethod()]
		public void GlobalPattern01Test()
		{
			RemotingMockService.AddGlobalMockingUri("*");
			Assert.IsTrue(RemotingMockService.IsUriToMock("http://localhost/service.rem"));
			using (RecorderManager.NewPlayBackSession("test", false))
			{
				Assert.IsTrue(RemotingMockService.IsUriToMock("http://localhost/service.rem"));
			}
			Assert.IsTrue(RemotingMockService.IsUriToMock("http://localhost/service.rem"));
			RemotingMockService.RemoveGlobalMockingUri("*");
			Assert.IsFalse(RemotingMockService.IsUriToMock("http://localhost/service.rem"));
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException))]
		public void GlobalPattern02Test()
		{
			RemotingMockService.AddMockingUri("*");
		}

		[TestMethod()]
		public void LocalPattern00Test()
		{
			Assert.IsFalse(RemotingMockService.IsUriToMock("http://localhost/service.rem"));
			using (RecorderManager.NewPlayBackSession("test", false))
			{
				RemotingMockService.AddMockingUri("*");
				Assert.IsTrue(RemotingMockService.IsUriToMock("http://localhost/service.rem"));
			}
			Assert.IsFalse(RemotingMockService.IsUriToMock("http://localhost/service.rem"));
		}

		[TestMethod()]
		public void LocalPattern01Test()
		{
			Assert.IsFalse(RemotingMockService.IsUriToMock("http://localhost/service.rem"));
			using (RecorderManager.NewPlayBackSession("test", false))
			{
				RemotingMockService.AddMockingUri("*");
				Assert.IsTrue(RemotingMockService.IsUriToMock("http://localhost/service.rem"));
				RemotingMockService.RemoveMockingUri("*");
				Assert.IsFalse(RemotingMockService.IsUriToMock("http://localhost/service.rem"));
			}
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException))]
		public void LocalPattern02Test()
		{
			using (RecorderManager.NewPlayBackSession("test", false))
			{
				RemotingMockService.AddGlobalMockingUri("*");
			}
		}

		[TestMethod()]
		public void UriPatternsTest()
		{
			using (RecorderManager.NewPlayBackSession("test", false))
			{
				RemotingMockService.AddMockingUri("http://*/*?id=5");
				Assert.IsTrue(RemotingMockService.IsUriToMock("http://localhost/service.rem?id=5"));
				Assert.IsFalse(RemotingMockService.IsUriToMock("http://localhost/service.rem?id=6"));
				Assert.IsFalse(RemotingMockService.IsUriToMock("tcp://localhost/service.rem?id=5"));
			}
		}
	}
}
