using System;
using System.Collections;

using Arebis.Mocking;

namespace Arebis.Testing.Mocking.Manual
{

	/// <summary>
	/// The mocker that handles manual mocking.
	/// </summary>
	public class ManualMocker : IMocker {

		private ArrayList expectedCalls = new ArrayList();
		private MockingProxy proxy;

		internal void Initialize(MockingProxy proxy) {
			this.proxy = proxy;
		}

		/// <summary>
		/// Expect the given call.
		/// </summary>
		public ManualMocker ExpectCall(IExpectedCall expectedCall) {
			EnqueueExpectedCall(expectedCall);
			return this;
		}

		/// <summary>
		/// Expect the given void method to be called.
		/// </summary>
		public ManualMocker Expect(string methodName) {
			ExpectCall(new ExpectAndReturnCall(methodName, null));
			return this;
		}

		/// <summary>
		/// Expect the given method to be called and return the given value.
		/// </summary>
		public ManualMocker ExpectAndReturn(string methodName, object returnValue) {
			ExpectCall(new ExpectAndReturnCall(methodName, returnValue));
			return this;
		}

		/// <summary>
		/// Expect the given method to be called, return the given value and
		/// reproduce the given output arguments.
		/// </summary>
		public ManualMocker ExpectAndReturn(string methodName, object returnValue, object[] outArgs) {
			ExpectCall(new ExpectAndReturnCall(methodName, returnValue, outArgs));
			return this;
		}

		/// <summary>
		/// Expect the given method to be called and throw the given exception.
		/// </summary>
		public ManualMocker ExpectAndThrow(string methodName, Exception exception) {
			ExpectCall(new ExpectAndThrowCall(methodName, exception));
			return this;
		}

		/// <summary>
		/// Expect the last expected call to be repeated multiple times.
		/// </summary>
		/// <param name="timesToRepeat">Times the last expected call is expected to be called.</param>
		public ManualMocker RepeatTimes(int timesToRepeat) {
			IExpectedCall lastCall = DequeueLastEnqueuedExpectedCall();
			for(int i=0; i<timesToRepeat; i++) EnqueueExpectedCall(lastCall);
			return this;
		}

		/// <summary>
		/// Check the arguments of the last expected call.
		/// </summary>
		/// <param name="arguments">The arguments to be expected.</param>
		/// <remarks>
		/// Provide IExpectedValue objects for arguments requiring special handling.
		/// Provide a value for all arguments, including the out arguments.
		/// The values for the out arguments are however ignored (can be null or anything else).
		/// </remarks>
		public ManualMocker WithArguments(params object[] arguments) {
			IExpectedCall lastCall = DequeueLastEnqueuedExpectedCall();
			EnqueueExpectedCall(new WithArgumentsCall(lastCall, arguments));
			return this;
		}

		/// <summary>
		/// The mock object for this mocker.
		/// </summary>
		public object Mock {
			get {
				return this.proxy.GetTransparentProxy();
			}
		}

		/// <summary>
		/// Validates the mocker. Checks that all expected calls have effectively been made.
		/// </summary>
		public void Validate() {
			if (expectedCalls.Count > 0)
				throw new ReplayMockException("Not all calls have been replayed on mock \"" + proxy.InstanceName + "\".");
		}

		private void EnqueueExpectedCall(IExpectedCall call) {
			expectedCalls.Add(call);
		}

		private IExpectedCall DequeueExpectedCall() {
			IExpectedCall call = (IExpectedCall)expectedCalls[0];
			expectedCalls.RemoveAt(0);
			return call;
		}
		
		private IExpectedCall DequeueLastEnqueuedExpectedCall() {
			IExpectedCall call = (IExpectedCall)expectedCalls[expectedCalls.Count-1];
			expectedCalls.RemoveAt(expectedCalls.Count-1);
			return call;
		}

		#region IMocker Members

		/// <summary>
		/// Implements IMocker.
		/// </summary>
		public void HandleCall(MockingProxy proxy, MockableCall call) {
			try {
				IExpectedCall expectedCall = DequeueExpectedCall();
				expectedCall.Replay(call);
			} catch (ArgumentOutOfRangeException) {
				throw new ReplayMockException(call, "Call not expected.");
			}
		}

		#endregion
	}


}
