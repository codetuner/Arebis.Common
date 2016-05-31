using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Runtime.Validation;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Validation
{
	[TestClass]
	public class PropertyAssertAttributeTests
	{
		[TestMethod]
		public void SuccessTest()
		{
			TestObject obj = new TestObject();

			obj.AssertValid();

			obj.IntOne = -3;
			obj.IntTwo = 75;
			obj.NintOne = -3;
			obj.NintTwo = 25;
			obj.DblOne = -3.3;
			obj.StrOne = "12/12/2012 12:12:12";
			obj.StrTwo = "Hello World";

			obj.AssertValid();

			obj.NintOne = null;
			obj.StrOne = null;

			obj.AssertValid();
		}

		[TestMethod]
		public void AssertNotNullTest()
		{
			TestObject obj = new TestObject();

			obj.AssertValid();

			obj.NintTwo = null;
			Assert.IsFalse(obj.IsValid(), "Validation failed to reject value");

			obj.NintTwo = 1;
			obj.AssertValid();

			obj.StrTwo = null;
			Assert.IsFalse(obj.IsValid(), "Validation failed to reject value");

			obj.StrTwo = "Foo";
			obj.AssertValid();

			obj.StrTwo = "       ";
			Assert.IsFalse(obj.IsValid(), "Validation failed to reject value");

			obj.StrTwo = "Bar";
			obj.AssertValid();
		}

		[TestMethod]
		public void AssertDoubleBetweenTest()
		{
			TestObject obj = new TestObject();

			obj.AssertValid();

			obj.DblOne = 12.0;
			Assert.IsFalse(obj.IsValid(), "Validation failed to reject value");

			obj.DblOne = -5.0;
			obj.AssertValid();
		}

		[TestMethod]
		public void AssertIntegerBetweenTest()
		{
			TestObject obj = new TestObject();

			obj.AssertValid();

			obj.IntOne = 8;
			Assert.IsFalse(obj.IsValid(), "Validation failed to reject value");

			obj.IntOne = 5;
			obj.AssertValid();

			obj.IntTwo = -1;
			Assert.IsFalse(obj.IsValid(), "Validation failed to reject value");

			obj.IntTwo = 100;
			obj.AssertValid();
		}

		[TestMethod]
		public void AssertNullableIntegerBetweenTest()
		{
			TestObject obj = new TestObject();

			obj.AssertValid();

			obj.NintOne = 8;
			Assert.IsFalse(obj.IsValid(), "Validation failed to reject value");

			obj.NintOne = 5;
			obj.AssertValid();

			obj.NintTwo = -1;
			Assert.IsFalse(obj.IsValid(), "Validation failed to reject value");

			obj.NintTwo = 100;
			obj.AssertValid();
		}

		[TestMethod]
		public void AssertStringSizeTest()
		{
			TestObject obj = new TestObject();

			obj.AssertValid();

			obj.StrTwo = "";
			Assert.IsFalse(obj.IsValid(), "Validation failed to reject value");

			obj.StrTwo = "123456789.123456789.1"; // 21 chars
			Assert.IsFalse(obj.IsValid(), "Validation failed to reject value");

			obj.StrTwo = "123456789.123456789."; // 20 chars
			obj.AssertValid();
		}

		[TestMethod]
		public void AssertRegexMatchTest()
		{
			TestObject obj = new TestObject();

			obj.AssertValid();

			obj.StrOne = "yesterday";
			Assert.IsFalse(obj.IsValid(), "Validation failed to reject value");

			obj.StrOne = "12/12/2012 12:12:12";
			obj.AssertValid();
		}
	}
}
