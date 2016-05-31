using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Reflection;

namespace Test.Arebis.Extensions.Tests.Arebis.Reflection
{
	[TestClass]
	public class CsLanguageInfoTests
	{
		[TestMethod]
		public void GetFiendlyNameTest()
		{
			ILanguageInfo subject = new CsLanguageInfo();
			subject.RegisterNamespace("System");
			subject.RegisterNamespace("System.Collections.Generic");
			subject.RegisterNamespace("System.IO", "SysIO");

			Assert.AreEqual("int", subject.GetFiendlyName(typeof(System.Int32)));
			Assert.AreEqual("string", subject.GetFiendlyName(typeof(System.String)));
			Assert.AreEqual("DateTime", subject.GetFiendlyName(typeof(System.DateTime)));
			Assert.AreEqual("Type", subject.GetFiendlyName(typeof(System.Type)));
			Assert.AreEqual("System.Globalization.CultureInfo", subject.GetFiendlyName(typeof(System.Globalization.CultureInfo)));
			Assert.AreEqual("SysIO.DriveInfo", subject.GetFiendlyName(typeof(System.IO.DriveInfo)));
			Assert.AreEqual("System.Collections.ArrayList", subject.GetFiendlyName(typeof(System.Collections.ArrayList)));
			Assert.AreEqual("Dictionary<string, SysIO.FileInfo>", subject.GetFiendlyName(typeof(System.Collections.Generic.Dictionary<System.String, System.IO.FileInfo>)));
			Assert.AreEqual("Dictionary<TKey, TValue>", subject.GetFiendlyName(typeof(System.Collections.Generic.Dictionary<,>)));
		}
	}
}
