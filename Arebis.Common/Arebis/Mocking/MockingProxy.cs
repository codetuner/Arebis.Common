using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Services;
using System.Runtime.Serialization;

namespace Arebis.Mocking {

	/// <summary>
	/// Proxy class that can have a mocker plugged in.
	/// </summary>
	public class MockingProxy : RealProxy {
	
		private string instanceName;
		private IMocker mocker;
		private Type serverType;

		/// <summary>
		/// Instantiates a mocking proxy with a given instance name.
		/// </summary>
		/// <param name="serverType">The type of object to proxy.</param>
		/// <param name="mocker">An instance that will mock the behaviour of the real instance.</param>
		/// <param name="instanceName">The name of the instance being mocked.</param>
		public MockingProxy(Type serverType, IMocker mocker, string instanceName) : base(serverType) {
			this.serverType = serverType;
			this.mocker = mocker;
			this.instanceName = instanceName;
		}

		/// <summary>
		/// The name of the instance. Instance names are used to identify mock objects,
		/// therefore, mock objects should have unique names.
		/// </summary>
		public string InstanceName {
			get {
				return this.instanceName;
			}
			set {
				this.instanceName = value;
			}
		}

		/// <summary>
		/// The type of object the mock should pretend to be.
		/// </summary>
		public Type ServerType {
			get {
				return this.serverType;
			}
		}

		/// <summary>
		/// Invokes the message on the mocker.
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
#if (!DEBUG)
		[System.Diagnostics.DebuggerHidden]
#endif
		public override IMessage Invoke(IMessage msg) {
			MockableCall call = new MockableCall(this, (IMethodCallMessage)msg);
			this.mocker.HandleCall(this, call);
			IMethodCallMessage mcm = msg as IMethodCallMessage;
			if (call.Exception != null) {
				return new ReturnMessage(call.Exception, mcm);
			} else if (call.IsConstructorCall) {
				return EnterpriseServicesHelper.CreateConstructionReturnMessage((IConstructionCallMessage)msg, (MarshalByRefObject)this.GetTransparentProxy());
			} else {
				return new ReturnMessage(call.ReturnValue, call.Args, call.GetOutParameters().Length, mcm.LogicalCallContext, mcm);
			}
		}

		/// <summary>
		/// Attaches the current proxy instance to the specified remote System.MarshalByRefObject.
		/// </summary>
		public new void AttachServer(MarshalByRefObject serverObject) {
			base.AttachServer(serverObject);
		}
	}
}
