using System;

using Arebis.Mocking;

namespace Arebis.Testing.Mocking.Manual
{

	/// <summary>
	/// Represents an expected call.
	/// </summary>
	public interface IExpectedCall {

		/// <summary>
		/// Replays the expected call.
		/// </summary>
		void Replay(MockableCall call);

	}
}
