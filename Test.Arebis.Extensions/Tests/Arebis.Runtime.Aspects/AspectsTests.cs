using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Arebis.Runtime.Aspects;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Extensibility
{
	[TestClass()]
	public class AspectsTests
	{
		[TestMethod()]
		public void Aspects01Test()
		{
			TestableAdviceAttribute.ResetCalls();

			DateTime t = (((Sample1.IFoo)new Sample1.YourFoo()).GetCurrentDateTime(3));

			Assert.AreEqual(new DateTime(2007, 1, 4), t);
			Assert.AreEqual(2, TestableAdviceAttribute.Calls.Count);

			// Check applied attributes:
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromIFooInterface"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromIFooMethod"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyFooClass"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyFooMethod"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyFooBarClass"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyGreatFooBarClass"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyGreatFooBarMethod"));
			Assert.IsTrue((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromYourFooClass"));
			Assert.IsTrue((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromYourFooMethod"));
		}

		[TestMethod()]
		public void Aspects02Test()
		{
			TestableAdviceAttribute.ResetCalls();

			DateTime t = (new Sample1.MyFoo().GetCurrentDateTime(3));

			Assert.AreEqual(new DateTime(2008, 1, 4), t);
			Assert.AreEqual(2, TestableAdviceAttribute.Calls.Count);

			// When calling an (implicit interface) implementation,
			// the CallContexts' method is the class-declared method:
			Assert.AreEqual(typeof(Sample1.MyFoo).GetMethod("GetCurrentDateTime"), TestableAdviceAttribute.Calls[1].Method);

			// Check applied attributes:
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromIFooInterface"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromIFooMethod"));
			Assert.IsTrue((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyFooClass"));
			Assert.IsTrue((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyFooMethod"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyFooBarClass"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyGreatFooBarClass"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyGreatFooBarMethod"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromYourFooClass"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromYourFooMethod"));
		}

		[TestMethod()]
		public void Aspects03Test()
		{
			TestableAdviceAttribute.ResetCalls();

			DateTime t = (new Sample1.MyFooBar().GetCurrentDateTime(3));

			Assert.AreEqual(new DateTime(2009, 1, 4), t);
			Assert.AreEqual(2, TestableAdviceAttribute.Calls.Count);

			// When calling an (implicit interface) implementation,
			// the CallContexts' method is the class-declared method:
			Assert.AreEqual(typeof(Sample1.MyFoo).GetMethod("GetCurrentDateTime"), TestableAdviceAttribute.Calls[1].Method);

			// Check applied attributes:
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromIFooInterface"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromIFooMethod"));
			Assert.IsTrue((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyFooClass"));
			Assert.IsTrue((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyFooMethod"));
			Assert.IsTrue((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyFooBarClass"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyGreatFooBarClass"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyGreatFooBarMethod"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromYourFooClass"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromYourFooMethod"));
		}
		
		[TestMethod()]
		public void Aspects04Test()
		{
			TestableAdviceAttribute.ResetCalls();

			DateTime t = (new Sample1.MyGreatFooBar().GetCurrentDateTime(3));

			Assert.AreEqual(new DateTime(2010, 1, 4), t);
			Assert.AreEqual(2, TestableAdviceAttribute.Calls.Count);

			// When calling an overriden (implicit interface) implementation,
			// the CallContexts' method is the overriden class-declared method:
			Assert.AreEqual(typeof(Sample1.MyGreatFooBar).GetMethod("GetCurrentDateTime"), TestableAdviceAttribute.Calls[1].Method);

			// Check applied attributes:
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromIFooInterface"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromIFooMethod"));
			Assert.IsTrue((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyFooClass"));
			Assert.IsTrue((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyFooMethod"));
			Assert.IsTrue((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyFooBarClass"));
			Assert.IsTrue((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyGreatFooBarClass"));
			Assert.IsTrue((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromMyGreatFooBarMethod"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromYourFooClass"));
			Assert.IsFalse((bool)((Sample1.BaseFoo)TestableAdviceAttribute.Calls[1].Instance).Markers.Contains("fromYourFooMethod"));
		}

		[TestMethod()]
		public void Aspects05Test()
		{
			TestableAdviceAttribute.ResetCalls();

			Sample1.MyFooBar instance = new Sample1.MyFooBar();
			instance.Tag = "helloTag";

			DateTime t = (instance.GetCurrentDateTime(3));

			Assert.AreEqual(new DateTime(2009, 1, 4), t);
			Assert.AreEqual(3, TestableAdviceAttribute.Calls.Count);

			// Check object instance:
			Assert.AreEqual("helloTag", ((Sample1.MyFoo)TestableAdviceAttribute.Calls[1].Instance).Tag);
		}

		[TestMethod()]
		public void Aspects06Test()
		{
			// Testing interception of methods on classes, implicite interface methods
			// and explicit interface methods.

			TestableAdviceAttribute.ResetCalls();

			Sample2.SampleClass instance = new Sample2.SampleClass(76);
			Sample2.SampleInterface iface = instance;

			instance.InterfaceMethodA();
			iface.InterfaceMethodA();
			iface.InterfaceMethodB();
			instance.InterfaceMethodC();

			foreach (ICallContext cc in TestableAdviceAttribute.Calls)
			{
				Console.WriteLine(
					"\r\n{0}_{1}\r\n  => {2}_{3} ({4})",
					cc.MethodCall.MethodBase.DeclaringType,
					cc.MethodCall.MethodBase.Name,
					cc.Method.DeclaringType,
					cc.Method.Name,
					cc.GetProperty("Name")
				);

				// Assert only advices on class operated:
				Assert.IsTrue(cc.GetProperty("Name").ToString().StartsWith("Class."));

				// Assert
				Assert.AreEqual(instance.Id, ((Sample2.SampleClass)cc.Instance).Id);
			}

			// All method calls except constructor intercepted:
			Assert.AreEqual(4, TestableAdviceAttribute.Calls.Count);

			// MethodCall.MethodBase of explicit interface implementation is interface's:
			Assert.AreEqual(typeof(Sample2.SampleInterface), TestableAdviceAttribute.Calls[2].MethodCall.MethodBase.DeclaringType);
			Assert.AreEqual("InterfaceMethodB", TestableAdviceAttribute.Calls[2].MethodCall.MethodBase.Name);

			// While Method of explicit interface implementation is classe's:
			Assert.AreEqual(typeof(Sample2.SampleClass), TestableAdviceAttribute.Calls[2].Method.DeclaringType);

			// So ALL Methods are classe's:
			Assert.AreEqual(typeof(Sample2.SampleClass), TestableAdviceAttribute.Calls[0].Method.DeclaringType);
			Assert.AreEqual(typeof(Sample2.SampleClass), TestableAdviceAttribute.Calls[1].Method.DeclaringType);
			Assert.AreEqual(typeof(Sample2.SampleClass), TestableAdviceAttribute.Calls[2].Method.DeclaringType);
			Assert.AreEqual(typeof(Sample2.SampleClass), TestableAdviceAttribute.Calls[3].Method.DeclaringType);

			// Also check methodnames:
			Assert.AreEqual("InterfaceMethodA", TestableAdviceAttribute.Calls[0].Method.Name);
			Assert.AreEqual("InterfaceMethodA", TestableAdviceAttribute.Calls[1].Method.Name);
			Assert.AreEqual(typeof(Sample2.SampleInterface).ToString() + ".InterfaceMethodB", TestableAdviceAttribute.Calls[2].Method.Name);
			Assert.AreEqual("InterfaceMethodC", TestableAdviceAttribute.Calls[3].Method.Name);
		}
	}
}
