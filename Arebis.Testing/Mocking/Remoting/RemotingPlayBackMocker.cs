using Arebis.Mocking;

namespace Arebis.Testing.Mocking.Remoting
{

	/// <summary>
	/// Remoting mocker.
	/// </summary>
	internal class RemotingPlayBackMocker : IMocker {
		
		/// <summary>
		/// Handles the call by playing it back.
		/// </summary>
		public void HandleCall(MockingProxy proxy, MockableCall call) {
			RecorderManager.PlayBackCall(call);
		}
	}
}
