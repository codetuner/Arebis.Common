namespace Arebis.Mocking {

	/// <summary>
	/// PlayBack mocker.
	/// </summary>
	internal class PlayBackMocker : IMocker {
		
		/// <summary>
		/// Handles the call by playing it back.
		/// </summary>
		public void HandleCall(MockingProxy proxy, MockableCall call) {
			RecorderManager.PlayBackCall(call);
		}
	}
}
