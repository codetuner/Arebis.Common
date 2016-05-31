using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using Sfmt = System.Runtime.Serialization.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Mocking;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking
{

	[TestClass()]
	public class MockableCallTests : _MockingTester
	{

		[TestMethod()]
		public void CreationTest()
		{
			MockingProxy callee = new MockingProxy(typeof(Sample.FooBar), null, "m1");
			MockableCall call = new MockableCall(callee, typeof(Sample.FooBar).GetMethod("SomeMethodWithInsAndOuts"), new object[] { 1, 2, null, 3 });
		}

		[TestMethod()]
		public void Serialization01Test()
		{
			MemoryStream buffer = new MemoryStream();
			IFormatter formatter = new global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

			// Create some call:
			ArrayList result = new ArrayList();
			result.Add("One");
			result.Add("Two");
			MockingProxy callee = new MockingProxy(typeof(Sample.FooBar), null, "m1");
			MockableCall call = new MockableCall(callee, typeof(Sample.FooBar).GetMethod("SomeMethodWithInsAndOuts"), new object[] { 1, 2, null, 3 });
			call.SetCallResult(result, new object[] { 7, 13 });

			// Serialize it:
			formatter.Serialize(buffer, call);
			buffer.Flush();

			// Deserialize it:
			buffer.Position = 0;
			MockableCall deserializedCall = (MockableCall)formatter.Deserialize(buffer);

			// Test result:
			Assert.IsNotNull(deserializedCall);
			Assert.IsFalse(deserializedCall.IsConstructorCall);
			Assert.IsNotNull(deserializedCall.Method);
			Assert.AreEqual("SomeMethodWithInsAndOuts", deserializedCall.Method.Name);
			Assert.AreEqual("Two", ((ArrayList)deserializedCall.ReturnValue)[1]);
			Assert.AreEqual(3, deserializedCall.InArgs[2]);
			Assert.AreEqual(13, deserializedCall.OutArgs[1]);
			Assert.IsTrue(deserializedCall.IsCompleted);
		}

		[TestMethod()]
		public void Serialization02Test()
		{

			// Create and initialize formatter:
			Sfmt.Binary.BinaryFormatter formatter = new Sfmt.Binary.BinaryFormatter();
			formatter.AssemblyFormat = Sfmt.FormatterAssemblyStyle.Simple;

			// Create DS:
			global::System.Data.DataSet ds = new global::System.Data.DataSet();
			global::System.Data.DataTable dt = ds.Tables.Add("SomeTable");
			global::System.Data.DataColumn dc1 = dt.Columns.Add("ID", typeof(global::System.Int32));
			ds.AcceptChanges();

			// Create MockableCall:
			MockingProxy callee = new MockingProxy(typeof(Sample.FooBar), null, "m1");
			MockableCall call = new MockableCall(callee, typeof(Sample.FooBar).GetMethod("SomeMethodWithInsAndOuts"), new object[] { 1, 2, null, 3 });

			// Set dataset as callresult:
			call.SetCallResult(ds);

			Assert.IsNotNull(call.ReturnValue, "Test setup failure, test could not even be run !");

			// Serialize call:
			MemoryStream buffer = new MemoryStream();
			formatter.Serialize(buffer, call);

			// Reset buffer:
			buffer.Flush();
			buffer.Position = 0;

			// Deserialize call:
			call = (MockableCall)formatter.Deserialize(buffer);

			// Verify results (expect returnValue to be non-null):
			Assert.IsNotNull(call);
			Assert.IsNotNull(call.ReturnValue, "ReturnValue is null, the old implementation issue has reoccured...");
			Assert.IsTrue(call.ReturnValue is global::System.Data.DataSet, "What the heck ? returnValue should have been a dataset !");
		}

		[TestMethod()]
		public void Serialization03Test()
		{

			// Create and initialize formatter:
			Sfmt.Soap.SoapFormatter formatter = new Sfmt.Soap.SoapFormatter();
			formatter.AssemblyFormat = Sfmt.FormatterAssemblyStyle.Simple;

			// Create DS:
			global::System.Data.DataSet ds = new global::System.Data.DataSet();
			global::System.Data.DataTable dt = ds.Tables.Add("SomeTable");
			global::System.Data.DataColumn dc1 = dt.Columns.Add("ID", typeof(global::System.Int32));
			ds.AcceptChanges();

			// Create MockableCall:
			MockingProxy callee = new MockingProxy(typeof(Sample.FooBar), null, "m1");
			MockableCall call = new MockableCall(callee, typeof(Sample.FooBar).GetMethod("SomeMethodWithInsAndOuts"), new object[] { 1, 2, null, 3 });

			// Set dataset as callresult:
			call.SetCallResult(ds);

			Assert.IsNotNull(call.ReturnValue, "Test setup failure, test could not even be run !");

			// Serialize call:
			MemoryStream buffer = new MemoryStream();
			formatter.Serialize(buffer, call);

			// Reset buffer:
			buffer.Flush();
			buffer.Position = 0;

			// Deserialize call:
			call = (MockableCall)formatter.Deserialize(buffer);

			// Verify results (expect returnValue to be non-null):
			Assert.IsNotNull(call);
			Assert.IsNotNull(call.ReturnValue, "ReturnValue is null, the old implementation issue has reoccured...");
			Assert.IsTrue(call.ReturnValue is global::System.Data.DataSet, "What the heck ? returnValue should have been a dataset !");
		}

		[TestMethod()]
		public void Serialization04Test()
		{
			MemoryStream buffer = new MemoryStream();
			Sfmt.Soap.SoapFormatter formatter = new Sfmt.Soap.SoapFormatter();
			formatter.AssemblyFormat = Sfmt.FormatterAssemblyStyle.Simple;

			// Create some call:
			MockingProxy callee = new MockingProxy(typeof(Sample.FooBar), null, "m1");
			MockableCall call = new MockableCall(callee, typeof(Sample.FooBar).GetMethod("IsItTrue"), new object[] { true });
			call.SetCallResult(true);

			// Serialize it:
			formatter.Serialize(buffer, call);
			buffer.Flush();

			// Deserialize it:
			buffer.Position = 0;
			MockableCall deserializedCall = (MockableCall)formatter.Deserialize(buffer);

			// Test result:
			Assert.IsNotNull(deserializedCall);
			Assert.IsFalse(deserializedCall.IsConstructorCall);
			Assert.IsNotNull(deserializedCall.Method);
			Assert.AreEqual("IsItTrue", deserializedCall.Method.Name);
			Assert.IsFalse(deserializedCall.ReturnValue is string, "ReturnValue of type bool was read back through soap serialization as a string.");
			Assert.IsTrue(deserializedCall.ReturnValue is bool);
			Assert.IsTrue(deserializedCall.InArgs[0] is bool);
			Assert.IsTrue((bool)deserializedCall.ReturnValue);
			Assert.IsTrue(deserializedCall.IsCompleted);
		}

		[TestMethod()]
		public void Serialization05Test()
		{
			MemoryStream buffer = new MemoryStream();
			Sfmt.Soap.SoapFormatter formatter = new Sfmt.Soap.SoapFormatter();
			formatter.AssemblyFormat = Sfmt.FormatterAssemblyStyle.Simple;

			// Create some call:
			MockingProxy callee = new MockingProxy(typeof(Sample.FooBar), null, "m1");
			MockableCall call = new MockableCall(callee, typeof(Sample.FooBar).GetMethod("SomeVoid"), new object[] { });
			call.SetCallResult();

			// Serialize it:
			formatter.Serialize(buffer, call);
			buffer.Flush();

			// Deserialize it:
			buffer.Position = 0;
			MockableCall deserializedCall = (MockableCall)formatter.Deserialize(buffer);

			// Test result:
			Assert.IsNotNull(deserializedCall);
			Assert.IsFalse(deserializedCall.IsConstructorCall);
			Assert.IsNotNull(deserializedCall.Method);
			Assert.AreEqual("SomeVoid", deserializedCall.Method.Name);
			Assert.IsTrue(deserializedCall.ReturnValue == null);
			Assert.IsTrue(deserializedCall.IsCompleted);
		}

		[TestMethod()]
		//[Ignore("This test failes as it uses a copy of the old, failing implementation. To be tested however with .NET 2.0 once")]
		public void Serialization99Test()
		{

			// Create and initialize formatter:
			Sfmt.Binary.BinaryFormatter formatter = new Sfmt.Binary.BinaryFormatter();
			formatter.AssemblyFormat = Sfmt.FormatterAssemblyStyle.Simple;

			// Create DS:
			global::System.Data.DataSet ds = new global::System.Data.DataSet();
			global::System.Data.DataTable dt = ds.Tables.Add("SomeTable");
			global::System.Data.DataColumn dc1 = dt.Columns.Add("ID", typeof(global::System.Int32));
			ds.AcceptChanges();

			// Create MockableCall:
			Support.OldImplMockableCall call = new Support.OldImplMockableCall();
			call.SetCallResult(ds);

			Assert.IsNotNull(call.ReturnValue, "Test setup failure, test could not even be run !");

			// Serialize call:
			MemoryStream buffer = new MemoryStream();
			formatter.Serialize(buffer, call);

			// Reset buffer:
			buffer.Flush();
			buffer.Position = 0;

			// Deserialize call:
			call = (Support.OldImplMockableCall)formatter.Deserialize(buffer);

			// Verify results:
			Assert.IsNotNull(call);
			Assert.IsNotNull(call.ReturnValueFromHolder, "ReturnValue 'late-retrieved' from call failed, this is really unexpected !!!");
			Assert.IsNull(call.ReturnValue, "ReturnValue 'early-retrieved 'should be null, which is a failure, but expected to be so !!!");
		}


		[TestMethod()]
		public void GetParametersTest()
		{
			// Create call:
			MockingProxy callee = new MockingProxy(typeof(Sample.FooBar), null, "m1");
			MockableCall call = new MockableCall(callee, typeof(Sample.FooBar).GetMethod("SomeMethodWithInsAndOuts"), new object[] { 1, 2, null, 3 });
			// Check parameters:
			Assert.AreEqual(3, call.GetInParameters().Length, "Input parameters count mismatch.");
			Assert.AreEqual(2, call.GetOutParameters().Length, "Output parameters count mismatch.");
		}

		[TestMethod()]
		public void IsInOutParameterTest()
		{
			// Create call:
			MockingProxy callee = new MockingProxy(typeof(Sample.FooBar), null, "m1");
			MockableCall call = new MockableCall(callee, typeof(Sample.FooBar).GetMethod("SomeMethodWithInsAndOuts"), new object[] { 1, 2, null, 3 });
			// Check in parameters:
			foreach (global::System.Reflection.ParameterInfo param in call.GetInParameters())
			{
				Assert.IsTrue(call.IsParameterIn(param.Position));
			}
			// Check out parameters:
			foreach (global::System.Reflection.ParameterInfo param in call.GetOutParameters())
			{
				Assert.IsTrue(call.IsParameterOut(param.Position));
			}
			// All params must be IN, OUT or IN/OUT:
			foreach (global::System.Reflection.ParameterInfo param in call.Method.GetParameters())
			{
				Assert.IsTrue(call.IsParameterIn(param.Position) || call.IsParameterOut(param.Position));
			}
		}


		[TestMethod()]
		public void ScenarioPlayTest()
		{

			using (RecorderManager.NewRecordingSession("test"))
			{

				// Make a callee, not really used but needed to record a constructor call:
				MockingProxy callee = new MockingProxy(typeof(Sample.Account), null, "m1");

				// Push some calls in the recorder:
				MockableCall lastcall;
				CurrentRecorder.RecordCall(lastcall = new MockableCall(callee, typeof(Sample.Account).GetConstructor(new Type[] { typeof(Sample.CurrencyUnit) }), null));
				lastcall.SetConstructionResult("acc");
				CurrentRecorder.RecordCall(lastcall = new MockableCall(callee, typeof(Sample.Account).GetMethod("Deposit"), null));
				lastcall.SetCallResult();
				CurrentRecorder.RecordCall(lastcall = new MockableCall(callee, typeof(Sample.Account).GetMethod("Withdraw"), null));
				lastcall.SetCallResult();
				CurrentRecorder.RecordCall(lastcall = new MockableCall(callee, typeof(Sample.Account).GetProperty("Balance").GetGetMethod(), null));
				lastcall.SetCallResult(10m);
			}

			using (RecorderManager.NewPlayBackSession("test", true))
			{
				// Register types to mock:
				MockService.AddTypeToMock(typeof(Sample.Account));

				// Play scenario:
				Sample.Account acc = new Sample.Account(Sample.CurrencyUnit.EUR);
				acc.Deposit(100m);
				acc.Withdraw(25m);
				Decimal balance = acc.Balance;

				// Checks:
				Assert.AreEqual(10m, balance); // Does not match the scenario, but the mocking result !
			}
		}
	}
}
