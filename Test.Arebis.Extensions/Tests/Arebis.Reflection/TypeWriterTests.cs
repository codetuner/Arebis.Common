using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Reflection;

namespace Test.Arebis.Extensions.Tests.Arebis.Reflection
{
	[TestClass]
	public class TypeWriterTests
	{
		[TestMethod]
		public void WriteTypeRegularTest()
		{
			Dictionary<string, IEnumerable<DateTime>[]> o = new Dictionary<string, IEnumerable<DateTime>[]>();
			Type t = o.GetType();

			TypeWriter writer = new TypeWriter();

			Console.WriteLine(writer.WriteType(t));
			Console.WriteLine(writer.WriteType(t.GetGenericTypeDefinition()));

			Assert.AreEqual("System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.Collections.Generic.IEnumerable`1[[System.DateTime, mscorlib]][], mscorlib]], mscorlib", writer.WriteType(t));
			Assert.AreEqual("System.Collections.Generic.Dictionary`2, mscorlib", writer.WriteType(t.GetGenericTypeDefinition()));
		}

		[TestMethod]
		public void WriteTypeMinimalTest()
		{
			Dictionary<string, IEnumerable<DateTime>[]> o = new Dictionary<string, IEnumerable<DateTime>[]>();
			Type t = o.GetType();

			TypeWriter writer = new TypeWriter();
			writer.WithAssembly = false;

			Console.WriteLine(writer.WriteType(t));
			Console.WriteLine(writer.WriteType(t.GetGenericTypeDefinition()));

			Assert.AreEqual("System.Collections.Generic.Dictionary`2[[System.String],[System.Collections.Generic.IEnumerable`1[[System.DateTime]][]]]", writer.WriteType(t));
			Assert.AreEqual("System.Collections.Generic.Dictionary`2", writer.WriteType(t.GetGenericTypeDefinition()));
		}

		[TestMethod]
		public void WriteTypeFullTest()
		{
			Dictionary<string, IEnumerable<DateTime>[]> o = new Dictionary<string, IEnumerable<DateTime>[]>();
			Type t = o.GetType();

			TypeWriter writer = new TypeWriter();
			writer.WithVersion = true;
			writer.WithCulture = true;
			writer.WithPublicKeyToken = true;

			Console.WriteLine(writer.WriteType(t));
			Console.WriteLine(writer.WriteType(t.GetGenericTypeDefinition()));

			Assert.AreEqual("System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Collections.Generic.IEnumerable`1[[System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]][], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", writer.WriteType(t));
			Assert.AreEqual("System.Collections.Generic.Dictionary`2, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", writer.WriteType(t.GetGenericTypeDefinition()));
		}

		[TestMethod]
		public void WriteAndReadTypeTest()
		{
			Dictionary<string, IEnumerable<DateTime>[]> o = new Dictionary<string, IEnumerable<DateTime>[]>();
			Type t = o.GetType();

			TypeWriter writer = new TypeWriter();

			Console.WriteLine(writer.WriteType(t));
			Console.WriteLine(writer.WriteType(t.GetGenericTypeDefinition()));

			Type t1 = Type.GetType(writer.WriteType(t));
			Type t2 = Type.GetType(writer.WriteType(t.GetGenericTypeDefinition()));

			Assert.AreEqual(t, t1);
			Assert.AreEqual(t.GetGenericTypeDefinition(), t2);
		}
	}
}
