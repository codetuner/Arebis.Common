using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Activation;
using System.Runtime.Serialization;
using Arebis.Mocking;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking.Support
{

	// http://support.microsoft.com/kb/818587/ ???

	[Serializable]
	public class OldImplMockableCall : ISerializable
	{

		private object returnValue;
		private object[] returnValueHolder;

		public OldImplMockableCall() { }

		public virtual object ReturnValue
		{
			get
			{
				return this.returnValue;
			}
		}

		public virtual object ReturnValueFromHolder
		{
			get
			{
				return this.returnValueHolder[0];
			}
		}

		public virtual void SetCallResult(object returnValue)
		{
			this.returnValue = returnValue;
		}

		private OldImplMockableCall(SerializationInfo info, StreamingContext context)
		{
			object[] values = (object[])info.GetValue("returnValue", typeof(object[]));
			this.returnValueHolder = values; // Keep de holder
			this.returnValue = values[0]; // But also "early-"unwrap the value
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// Wrap returnValue in a holder:
			info.AddValue("returnValue", new object[] { this.returnValue });
		}
	}

	[Serializable]
	internal class ArgHolder
	{
		private object value;

		internal ArgHolder(object value)
		{
			this.value = value;
		}

		internal virtual object Value
		{
			get
			{
				return this.value;
			}
		}
	}
}
