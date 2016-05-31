using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Security;

namespace Arebis.Runtime.Aspects
{
    /// <summary>
    /// ICallContext implementation.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    internal class CallContext : ICallContext
    {

        private Dictionary<string, object> _properties;
        private IMethodCallMessage _callMessage;
        private MethodBase _methodBase;
        private object _instance;
        private IMethodReturnMessage _returnMessage;

        internal CallContext(IMethodCallMessage callMessage, MethodBase methodBase, object instance)
        {
            this._callMessage = callMessage;
            this._methodBase = methodBase;
            this._instance = instance;
        }

        internal void SetPropertyContainer(Dictionary<string, object> properties)
        {
            this._properties = properties;
        }

        internal void SetReturnMessage(IMethodReturnMessage returnMessage)
        {
            this._returnMessage = returnMessage;
        }

        /// <summary>
        /// Retrieves a custom property.
        /// </summary>
        public object GetProperty(string name)
        {
            return this.GetProperty(name, null);
        }

        /// <summary>
        /// Retrieves a custom property.
        /// </summary>
        public object GetProperty(string name, object defaultValue)
        {
            try
            {
                return this._properties[name];
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Sets a custom property.
        /// </summary>
        public void SetProperty(string name, object value)
        {
            this._properties[name] = value;
        }

        /// <summary>
        /// The instance on which the method is called (null for constructor method calls).
        /// </summary>
        public Object Instance
        {
            get { return this._instance; }
        }

        /// <summary>
        /// The method that is being called.
        /// </summary>
        public MethodBase Method
        {
            get { return this._methodBase; }
        }

        /// <summary>
        /// The IMethodCallMessage object for the current method call.
        /// </summary>
        public IMethodCallMessage MethodCall
        {
            get { return this._callMessage; }
        }

        /// <summary>
        /// The IMethodReturnMessage object for the current method call.
        /// (Only available from within the AfterCall.)
        /// </summary>
        public IMethodReturnMessage ReturnMessage
        {
            get { return this._returnMessage; }
        }

        /// <summary>
        /// The return value of the call (null for void methods).
        /// (Only available from within the AfterCall.)
        /// </summary>
        public object ReturnValue
        {
            [SecuritySafeCritical]
            get
            {
                if (this._returnMessage is IConstructionReturnMessage)
                    // Querying internal GetObject() method to avoid call to MarshallInternal which
                    // also would make the object non-garbagecollectable:
                    return this._returnMessage.GetType().GetMethod("GetObject", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this._returnMessage, null);
                else if (this._returnMessage is IMethodReturnMessage)
                    return this._returnMessage.ReturnValue;
                else
                    return null;
            }
        }

        /// <summary>
        /// The exception raised in the call, or null if call succeeded.
        /// (Only available from within the AfterCall.)
        /// </summary>
        public Exception Exception
        {
            [SecuritySafeCritical]
            get { return this._returnMessage.Exception; }
        }

        /// <summary>
        /// Whether the call failed (raised an exception).
        /// (Only available from within the AfterCall.)
        /// </summary>
        public bool CallFailed
        {
            get { return !this.CallSucceeded; }
        }

        /// <summary>
        /// Whether the call succeeded (did not raise an exception).
        /// (Only available from within the AfterCall.)
        /// </summary>
        public bool CallSucceeded
        {
            get { return this.Exception == null; }
        }

        /// <summary>
        /// Overwrites the return value of the method.
        /// (Only callable from within the AfterCall.)
        /// </summary>
        public void SetReturnValue(object newReturnValue)
        {
            throw new NotSupportedException("Changing the return value from within an advisor is not supported by the current implementation.");
        }

        /// <summary>
        /// Forces the method call to return the given exception.
        /// (Only callable from within the AfterCall.)
        /// </summary>
        [SecuritySafeCritical]
        public void SetException(Exception newException)
        {
            this._returnMessage = new ReturnMessage(newException, this._callMessage);
        }
    }
}
