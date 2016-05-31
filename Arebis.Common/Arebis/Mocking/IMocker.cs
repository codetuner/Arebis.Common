using System;

namespace Arebis.Mocking
{

	/// <summary>
	/// Defines custom mockers.
	/// </summary>
	public interface IMocker
	{

		/// <summary>
		/// Called whenever the mock should handle a call.
		/// </summary>
		void HandleCall(MockingProxy proxy, MockableCall call);

	}
}
