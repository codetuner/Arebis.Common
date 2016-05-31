using System;
using System.Runtime.Remoting.Messaging;

namespace Arebis.Mocking
{

	/// <summary>
	/// Defines the recorder for recording and playback of MockableCalls.
	/// </summary>
	public interface IRecorder
	{
		/// <summary>
		/// Called to mark the beginning of a new playback session.
		/// </summary>
		/// <param name="recordingName">The name of the recording to playback.</param>
		void BeginPlayBack(string recordingName);

		/// <summary>
		/// Called to playback a methodcall.
		/// </summary>
		/// <remarks>Typically, the implementation of this method would check if this
		/// method is expected in the recording sequence, and if so, provide the
		/// returnvalue, output arguments and/or exception to replay the call.</remarks>
		void PlayBackCall(MockableCall call);

		/// <summary>
		/// Called to mark the end of a playback session.
		/// </summary>
		void EndPlayBack();

		/// <summary>
		/// Called at the end of recording playback, to validate the playback.
		/// </summary>
		/// <remarks>The ValidatePlayBack method is to be called after all calls
		/// have been played, but before EndPlayBack is called. Typically, it's
		/// implementation will check that the recording was played completely.
		/// If not, it should throw a ReplayException making the playback to fail.
		/// </remarks>
		void ValidatePlayBack();

		/// <summary>
		/// Called to mark the beginning of a new recording session.
		/// </summary>
		/// <param name="recordingName">The name of the recording to record.</param>
		void BeginRecording(string recordingName);

		/// <summary>
		/// Called to record a call.
		/// </summary>
		/// <remarks>Typically, the implementation of this method would store information
		/// about this call, such as call identity, method name and signature, input 
		/// argument values, as well as returnvalue and output argument values, or raised
		/// exception to replay the call later on.</remarks>
		void RecordCall(MockableCall call);

		/// <summary>
		/// Calles to mark the end of a recording session.
		/// </summary>
		void EndRecording();

		/// <summary>
		/// Generates a new instance name for an object of the given type.
		/// </summary>
		/// <param name="objectType">The type of object to get a new instance name.</param>
		/// <remarks>
		/// This call is only valid during recording sessions.
		/// </remarks>
		string GetNextInstanceName(Type objectType);

	}
}
