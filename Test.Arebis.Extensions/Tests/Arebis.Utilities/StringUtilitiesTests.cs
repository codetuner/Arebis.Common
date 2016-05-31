using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Extensions;

namespace Arebis.Extensions.Tests.Arebis.Utilities
{
	[TestClass()]
	public class StringUtilitiesTests
	{
		[TestMethod()]
		public void Like01Test()
		{
			Assert.IsTrue(StringExtension.Like(@"", @"*"));
			Assert.IsTrue(StringExtension.Like(@"Foo", @"*"));
			Assert.IsTrue(StringExtension.Like(@"Foo", @"foo*"));
			Assert.IsTrue(StringExtension.Like(@"FooBar", @"foo*"));
			Assert.IsTrue(StringExtension.Like(@"FooBar", @"foob?r"));
			Assert.IsFalse(StringExtension.Like(@"FooBar", @"fooba?r"));
		}
	}
}
