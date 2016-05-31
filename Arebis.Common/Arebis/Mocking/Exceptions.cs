using System;

namespace Arebis.Mocking {

	/// <summary>
	/// Base exception for exceptions raised by the mock framework.
	/// </summary>
	public class MockException : Exception {
	
		/// <summary>
		/// Constructs a MockException.
		/// </summary>
		public MockException() : base() {}

		/// <summary>
		/// Constructs a MockException.
		/// </summary>
		public MockException(string message) : base(message) {}
		
		/// <summary>
		/// Constructs a MockException.
		/// </summary>
		public MockException(string message, Exception innerException) : base(message, innerException) {}

	}

	/// <summary>
	/// Exception indicating the replay of a recording failed, probably due to a mismatch
	/// between the expected calls and the effective calls.
	/// </summary>
	public class ReplayMockException : MockException {

		private MockableCall failedCall;
	
		/// <summary>
		/// Constructs a ReplayMockException.
		/// </summary>
		public ReplayMockException(string message) : this(null, message) {
		}

		/// <summary>
		/// Constructs a ReplayMockException for a given failed call.
		/// </summary>
		public ReplayMockException(MockableCall failedCall, string message) : base(message) {
			this.failedCall = failedCall;
		}

		/// <summary>
		/// The call that failed to replay.
		/// </summary>
		public MockableCall FailedCall {
			get {
				return this.failedCall;
			}
		}

	}

	/// <summary>
	/// Exception indicating an object passed to a mocked call is of an unsupported type.
	/// </summary>
	public class TypeMockException : MockException {

		/// <summary>
		/// Constructs a TypeMockException.
		/// </summary>
		public TypeMockException(Type t) : base("Instances of type \"" + t.FullName + "\" can not be in mocked calls." ) {}

	}


}
