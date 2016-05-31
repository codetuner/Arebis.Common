using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Runtime.Serialization;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Serialization
{
	[TestClass()]
	public class SurrogatesRegisterTests
	{
		[TestMethod]
		public void MultipleSurrogates01Test()
		{
			SurrogatesRegister subject = new SurrogatesRegister();

			subject.RegisterSurrogate(1.0, typeof(String), new TestSurrogate());
			subject.RegisterSurrogate(1.0, typeof(Decimal), new TestSurrogate());

			Assert.AreEqual(2, subject.ListRegistrations().Count);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void MultipleSurrogates02Test()
		{
			SurrogatesRegister subject = new SurrogatesRegister();

			subject.RegisterSurrogate(1.0, typeof(String), new TestSurrogate());
			subject.RegisterSurrogate(2.4, typeof(String), new TestSurrogate());
		}

		[TestMethod]
		public void MultipleSurrogates03Test()
		{
			SurrogatesRegister subject = new SurrogatesRegister();

			subject.RegisterSurrogate(1.0, typeof(String), new TestSurrogate());
			subject.UnregisterSurrogate(2.0, typeof(String));
			subject.RegisterSurrogate(2.4, typeof(String), new TestSurrogate());

			Assert.AreEqual(2, subject.ListRegistrations().Count);
		}

		[TestMethod]
		public void MultipleSurrogates04Test()
		{
			SurrogatesRegister subject = new SurrogatesRegister();

			subject.RegisterSurrogate(1.0, typeof(String), new TestSurrogate());
			subject.UnregisterSurrogate(2.4, typeof(String));
			subject.RegisterSurrogate(2.4, typeof(String), new TestSurrogate());

			Assert.AreEqual(2, subject.ListRegistrations().Count);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullSurrogateTest()
		{
			SurrogatesRegister subject = new SurrogatesRegister();

			subject.RegisterSurrogate(1.0, typeof(String), null);
		}

		#region Test Surrogate

		class TestSurrogate : global::System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj, global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context)
			{
				throw new NotImplementedException("The method or operation is not implemented.");
			}

			public object SetObjectData(object obj, global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context, global::System.Runtime.Serialization.ISurrogateSelector selector)
			{
				throw new NotImplementedException("The method or operation is not implemented.");
			}
		}

		#endregion Test Surrogate
	}
}
