using System;

using Arebis.Mocking;

namespace Arebis.Testing.Mocking.Manual
{

	/// <summary>
	/// Base class for several IExpectedCall implementations.
	/// </summary>
	public abstract class ExpectedCallBase : IExpectedCall {

		private string methodName;

		/// <summary>
		/// Constructs an ExpectedBase.
		/// </summary>
		/// <param name="methodName">The name of the method to be expected.</param>
		public ExpectedCallBase(string methodName) {
			this.methodName = methodName;
		}

		/// <summary>
		/// Validates a call before replaying it. Checks that the call matches the expected
		/// call. If not so, raises a ReplayMockException.
		/// </summary>
		public virtual void Validate(MockableCall call) {
			if (!this.methodName.Equals(call.Method.Name))
				throw new ReplayMockException(call, "Expected call name does not match received call.\r\nCall expected: " + methodName + "\r\nCall received: " + call.Method.Name);
		}

		/// <summary>
		/// Replays the expected call.
		/// </summary>
		public virtual void Replay(MockableCall call) {
			Validate(call);
		}
	}

	/// <summary>
	/// Represents an expected call that returns a value.
	/// </summary>
	public class ExpectAndReturnCall : ExpectedCallBase {

		private object returnValue;
		private object[] outArgs;

		/// <summary>
		/// Constructs an ExpectAndReturnCall.
		/// </summary>
		/// <param name="methodName">The name of the method to be expected.</param>
		/// <param name="returnValue">The value to be returned.</param>
		public ExpectAndReturnCall(string methodName, object returnValue) : this(methodName, returnValue, new object[] {}) {}

		/// <summary>
		/// Constructs an ExpectAndReturnCall.
		/// </summary>
		/// <param name="methodName">The name of the method to be expected.</param>
		/// <param name="returnValue">The value to be returned.</param>
		/// <param name="outArgs">Output arguments to be reproduced.</param>
		public ExpectAndReturnCall(string methodName, object returnValue, object[] outArgs) : base(methodName) {
			this.returnValue = returnValue;
			this.outArgs = outArgs;
		}

		/// <summary>
		/// Replays the expected call by returning the expected returnvalue.
		/// </summary>
		public override void Replay(MockableCall call) {
			base.Replay(call);
			call.SetCallResult(returnValue, outArgs);
		}
 	}

	/// <summary>
	/// Represents an expected call that throws an exception.
	/// </summary>
	public class ExpectAndThrowCall : ExpectedCallBase {

		private Exception exception;

		/// <summary>
		/// Constructs an ExpectAndThrowCall.
		/// </summary>
		/// <param name="methodName">The name of the method to be expected.</param>
		/// <param name="exception">The exception to be thrown.</param>
		public ExpectAndThrowCall(string methodName, Exception exception) : base(methodName) {
			this.exception = exception;
		}

		/// <summary>
		/// Replays the expected call by throwing the exception.
		/// </summary>
		public override void Replay(MockableCall call) {
			base.Replay(call);
			call.SetExceptionResult(exception);
		}
	}

	/// <summary>
	/// An IExpectedCall wrapper that checks the arguments of the expected call.
	/// </summary>
	public class WithArgumentsCall : IExpectedCall {

		private IExpectedCall innerCall;
		private object[] arguments;

		/// <summary>
		/// Constructs a WithArgumentsCall wrapper.
		/// </summary>
		/// <param name="innerCall">The IExpectedCall to wrap, for which the arguments should be checked.</param>
		/// <param name="arguments">The argument values to expect. Provide IExpectedValue instances for special checkings.</param>
		public WithArgumentsCall(IExpectedCall innerCall, object[] arguments) {
			this.innerCall = innerCall;
			this.arguments = arguments;
		}

		/// <summary>
		/// Replays the expected call and checks its arguments.
		/// </summary>
		public void Replay(MockableCall call) {
			// Check argument count:
			if (arguments.Length != call.Method.GetParameters().Length) {
				throw new ReplayMockException(call, "Call to method \"" + call.Method.Name + "\" expected wrong number of arguments.");
			}
			// Check arguments individually:
			int i = -1;
			foreach(System.Reflection.ParameterInfo pinfo in call.GetInParameters()) {
				i++;
				if (pinfo.IsOut)
					continue; // Skip output parameters
				if ((arguments[pinfo.Position] == null) && (call.InArgs[i] == null))
					continue; // OK if both NULL
				if ((arguments[pinfo.Position] == null) || (call.InArgs[i] == null))
					throw new ReplayMockException(call, "Argument \"" + pinfo.Name + "\" of method \"" + call.MethodSignature + "\" has a different value than expected.");
				if (arguments[pinfo.Position] is IExpectedValue)
					if ((arguments[pinfo.Position] as IExpectedValue).MatchesExpectation(call.InArgs[i]))
						continue;
					else
						throw new ReplayMockException(call, "Argument \"" + pinfo.Name + "\" of method \"" + call.MethodSignature + "\" has a different value than expected.");
				if (!arguments[pinfo.Position].Equals(call.InArgs[i])) 
					throw new ReplayMockException(call, "Argument \"" + pinfo.Name + "\" of method \"" + call.MethodSignature + "\" has a different value than expected.");
			}
			// If all passed, replay the call:
			innerCall.Replay(call);
		}
	}
}
