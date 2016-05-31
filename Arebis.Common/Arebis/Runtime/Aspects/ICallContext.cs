using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace Arebis.Runtime.Aspects
{
	/// <summary>
	/// A Context representing the current methodcall and in which an advice can store
	/// custom properties accross before and after calls.
	/// </summary>
	public interface ICallContext
	{
		/// <summary>
		/// Retrieves a custom property.
		/// </summary>
		object GetProperty(string name);

		/// <summary>
		/// Retrieves a custom property.
		/// </summary>
		object GetProperty(string name, object defaultValue);

		/// <summary>
		/// Sets a custom property.
		/// </summary>
		void SetProperty(string name, object value);

		/// <summary>
		/// The instance on which the method is called (null for constructor method calls).
		/// </summary>
		Object Instance
		{
			get;
		}

		/// <summary>
		/// The method that is being called.
		/// </summary>
		MethodBase Method
		{
			get;
		}

		/// <summary>
		/// The IMethodCallMessage object for the current method call.
		/// </summary>
		IMethodCallMessage MethodCall
		{
			get;
		}

		/// <summary>
		/// The IMethodReturnMessage object for the current method call.
		/// (Only available from within the AfterCall.)
		/// </summary>
		IMethodReturnMessage ReturnMessage
		{
			get;
		}

		/// <summary>
		/// The return value of the call (null for void methods).
		/// (Only available from within the AfterCall.)
		/// </summary>
		object ReturnValue
		{
			get;
		}

		/// <summary>
		/// The exception raised in the call, or null if call succeeded.
		/// (Only available from within the AfterCall.)
		/// </summary>
		Exception Exception
		{
			get;
		}

		/// <summary>
		/// Whether the call failed (raised an exception).
		/// (Only available from within the AfterCall.)
		/// </summary>
		bool CallFailed
		{
			get;
		}

		/// <summary>
		/// Whether the call succeeded (did not raise an exception).
		/// (Only available from within the AfterCall.)
		/// </summary>
		bool CallSucceeded
		{
			get;
		}

		/// <summary>
		/// Overwrites the return value of the method.
		/// (Only callable from within the AfterCall.)
		/// </summary>
		void SetReturnValue(object newReturnValue);

		/// <summary>
		/// Forces the method call to return the given exception.
		/// (Only callable from within the AfterCall.)
		/// </summary>
		void SetException(Exception newException);
	}
}
