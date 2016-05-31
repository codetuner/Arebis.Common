using System;
using System.Collections;

using Arebis.Mocking;

namespace Arebis.Testing.Mocking.Manual
{

	/// <summary>
	/// Represents a manual mock session.
	/// </summary>
	public class ManualMockSession : IDisposable {

		private ArrayList mockers = new ArrayList();
		private bool autoValidating = false;

		/// <summary>
		/// Constructs a regular ManualMockSession.
		/// </summary>
		public ManualMockSession() {}

		/// <summary>
		/// Constructs a ManualMockSession that used with using can validate automatically.
		/// </summary>
		public ManualMockSession(bool validateAtEnd) {
			this.autoValidating = validateAtEnd;
		}

		/// <summary>
		/// Creates and returns a mocker for the given object type.
		/// </summary>
		/// <param name="type">The object type to mock. This can be either an interface, or a type derived from MarshalByRef.</param>
		/// <param name="name">The name uniquely identifying the instance.</param>
		public virtual ManualMocker Mock(Type type, string name) {
			try {
				ManualMocker mocker = new ManualMocker();
				MockingProxy proxy = new MockingProxy(type, mocker, name);
				mocker.Initialize(proxy);
				mockers.Add(mocker);
				return mocker;
			} catch (ArgumentException ex) {
				throw new MockException("Type to mock must be an interface or derive from MarshalByRef.", ex);
			}
		}

		/// <summary>
		/// Validates the session by checking that all expected calls have been made.
		/// </summary>
		public virtual void ValidateSession() {
			foreach(ManualMocker mocker in mockers)
				mocker.Validate();
		}

		/// <summary>
		/// Disposes the session.
		/// </summary>
		public void Dispose() {
			if (autoValidating) ValidateSession();
		}
	}
}
