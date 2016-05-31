using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Services;

namespace Arebis.Mocking
{

	/// <summary>
	/// Recording mocker.
	/// </summary>
	internal class RecordingMocker : IMocker {

		private object serverInstance;

		/// <summary>
		/// Creates a new RecordingMocker isntance.
		/// </summary>
		/// <param name="serverInstance">The object for which calls are to be recorded.</param>
		public RecordingMocker(object serverInstance) {
			this.serverInstance = serverInstance;
		}

		/// <summary>
		/// Handles the call by playing it on the server, and recording its results.
		/// </summary>
#if (!DEBUG)
		[System.Diagnostics.DebuggerHidden]
#endif
		public void HandleCall(MockingProxy proxy, MockableCall call) {
			using (new RecorderManager.Lock()) {
				call.SetResult(InvokeOnServer(proxy, call));
				RecorderManager.RecordCall(call);
			}
		}

#if (!DEBUG)
		[System.Diagnostics.DebuggerHidden]
#endif
		private IMethodReturnMessage InvokeOnServer(MockingProxy proxy, MockableCall call) {
			IMessage request = call.OriginalCall;
			IMethodCallMessage mcm = request as IMethodCallMessage;
			IConstructionCallMessage ccm = request as IConstructionCallMessage;
			if(ccm != null) {
				try {
					// Attach server instance:
					//   (we do it only for MarshalByRefObjects as it's a requirement, don't really
					//    know however what's the added value of attachingToServer... it also works
					//    well without doing this)
					if (this.serverInstance is MarshalByRefObject) proxy.AttachServer((MarshalByRefObject)this.serverInstance);
					// Call instance constructor:
					RemotingServices.GetRealProxy(serverInstance).InitializeServerObject(ccm);
					// Build response:
					return MockingTools.BuildReturnMessage(proxy.GetTransparentProxy(), ccm.Args, mcm);
				} catch (Exception ex) {
					// Build response:
					return MockingTools.BuildReturnMessage(ex, mcm);
				}
			} else {
				try {
					// Invoke instance method:
					object[] args = new object[mcm.ArgCount];
					mcm.Args.CopyTo(args, 0);
					object result = mcm.MethodBase.Invoke(serverInstance, args);
					// Build response:
					return MockingTools.BuildReturnMessage(result, args, mcm);
				} catch (TargetInvocationException ex) {
					// Build response:
					return MockingTools.BuildReturnMessage(ex.InnerException, mcm);
				}
			}
		}
	}
}
