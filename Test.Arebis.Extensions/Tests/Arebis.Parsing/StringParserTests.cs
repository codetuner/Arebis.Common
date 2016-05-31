using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Parsing;
using System.Collections.Specialized;

namespace Arebis.Extensions.Tests.Arebis.Parsing
{
	[TestClass]
	public class StringParserTests
	{
		[TestMethod]
		public void ParseNameValuesNullTest()
		{
			NameValueCollection result = StringParser.ParseNameValues(null);
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void ParseNameValuesEmptyTest()
		{
			NameValueCollection result = StringParser.ParseNameValues("");
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void ParseNameValuesWordTest()
		{
			NameValueCollection result = StringParser.ParseNameValues("Foo");
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void ParseNameValuesValue01Test()
		{
			NameValueCollection result = StringParser.ParseNameValues("Name=\"James\";Surname=\"Bond\";Identifier=007;Boss=;Age=35");
			Assert.AreEqual(5, result.Count);
			Assert.AreEqual("James", result["Name"]);
			Assert.AreEqual("Bond", result["Surname"]);
			Assert.AreEqual("007", result["identifier"]);
			Assert.AreEqual("", result["boss"]);
			Assert.AreEqual("35", result["AGE"]);
		}

		[TestMethod]
		public void ParseNameValuesValue02Test()
		{
			NameValueCollection result = StringParser.ParseNameValues("Name=\"John\"; Preferences=\"left=45;mid=12\"");
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("John", result["Name"]);
			Assert.AreEqual("John", result["name"]);
			Assert.AreEqual("left=45;mid=12", result["preferences"]);
		}

		[TestMethod]
		public void SplitByTokens01Test()
		{
			string[] result = StringParser.SplitByTokens("My name is <%= name %>.", new string[] {"<%=", "%>"}, true, true, StringComparison.InvariantCulture);
			Assert.AreEqual("My name is ", result[0]);
			Assert.AreEqual("<%=", result[1]);
			Assert.AreEqual(" name ", result[2]);
			Assert.AreEqual("%>", result[3]);
			Assert.AreEqual(".", result[4]);
			Assert.AreEqual(5, result.Length);
		}

		[TestMethod]
		public void SplitByTokens02Test()
		{
			string[] result = StringParser.SplitByTokens("<%= Date.Now %> is <%=value%><%= excl %>", new string[] { "<%=", "%>" }, false, true, StringComparison.InvariantCulture);
			Assert.AreEqual("", result[0]);
			Assert.AreEqual(" Date.Now ", result[1]);
			Assert.AreEqual(" is ", result[2]);
			Assert.AreEqual("value", result[3]);
			Assert.AreEqual("", result[4]);
			Assert.AreEqual(" excl ", result[5]);
			Assert.AreEqual("", result[6]);
			Assert.AreEqual(7, result.Length);
		}
	}
}
