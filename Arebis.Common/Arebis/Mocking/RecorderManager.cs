using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;

namespace Arebis.Mocking {

	/// <summary>
	/// Possible states of a recorder.
	/// </summary>
	public enum RecorderState {
		/// <summary>Not playing or recording</summary>
		None = 0,

		/// <summary>Playing</summary>
		PlayBack,

		/// <summary>Recording</summary>
		Recording
	}

	/// <summary>
	/// The RecorderManager provides static access to, and control of the active Recorder.
	/// </summary>
	public sealed class RecorderManager {
	
		private static RecorderState action = RecorderState.None;
		private static IRecorder recorder;
		private static int recordNestLevel = 0;
		private static string currentRecordingName;

		/// <summary>
		/// Raised on the beginning of a PlayBack session.
		/// The recorder instance is passed as sender.
		/// </summary>
		public static event EventHandler OnBeginPlayback;

		/// <summary>
		/// Raised on the end of a PlayBack session.
		/// The recorder instance is passed as sender.
		/// </summary>
		public static event EventHandler OnEndPlayback;
		
		/// <summary>
		/// Raised on the beginning of a Recording session.
		/// The recorder instance is passed as sender.
		/// </summary>
		public static event EventHandler OnBeginRecording;
		
		/// <summary>
		/// Raised on the end of a Recording session.
		/// The recorder instance is passed as sender.
		/// </summary>
		public static event EventHandler OnEndRecoring;

		private RecorderManager() {
			throw new InvalidOperationException("RecorderManager is not to be instantiated.");
		}

		/// <summary>
		/// The current state of the recorder.
		/// </summary>
		public static RecorderState Action {
			get {
				return action;
			}
		}

		/// <summary>
		/// The recordingName currently being played back or recorded.
		/// </summary>
		public static string CurrentRecordingName {
			get {
				return currentRecordingName;
			}
		}

		/// <summary>
		/// The currently installed recorder.
		/// </summary>
		public static IRecorder Recorder {
			get {
				return recorder;
			}
			set {
				if (IsPlaying) throw new InvalidOperationException("Cannot switch recorder while busy playing.");
				if (IsRecording) throw new InvalidOperationException("Cannot switch recorder while busy recording.");
				recorder = value;
			}
		}

		/// <summary>
		/// Whether the current recorder is busy playing or recording.
		/// </summary>
		public static bool IsMocking {
			get { return action != RecorderState.None; }
		}

		/// <summary>
		/// Whether the current recorder is busy playing a recording.
		/// </summary>
		public static bool IsPlaying {
			get { return action == RecorderState.PlayBack; }
		}

		/// <summary>
		/// Whether the current recorder is busy recording a recording.
		/// </summary>
		public static bool IsRecording {
			get { return action == RecorderState.Recording; }
		}

		/// <summary>
		/// Returns a recording session instance.
		/// </summary>
		/// <remarks>To be used in a using block.</remarks>
		public static IDisposable NewRecordingSession(string recordingName) {
			return new RecordingSession(recordingName);
		}

		/// <summary>
		/// Initiates a recording session.
		/// </summary>
		public static void BeginRecording(string recordingName) {
			if (recorder == null) throw new InvalidOperationException("A recorder needs to be registered to the recorder before starting a RecorderManager session.");
			if (action != RecorderState.None) throw new InvalidOperationException("Recorder is already busy playing or recording.");
			currentRecordingName = recordingName;
			MockService.ResetSession();
			recorder.BeginRecording(recordingName);
			action = RecorderState.Recording;
			if (OnBeginRecording != null) OnBeginRecording(recorder, EventArgs.Empty);
		}

		/// <summary>
		/// Records the given call.
		/// </summary>
		public static void RecordCall(MockableCall call) {
			if (recordNestLevel == 1) {
				recorder.RecordCall(call);
			} else if (recordNestLevel == 0) {
				throw new InvalidOperationException("RecorderManager should be locked before recording a call.");
			}
		}

		/// <summary>
		/// Currently a call is being recorded.
		/// </summary>
		/// <remarks>
		/// Whenever this property is true, the code being executed is inside a
		/// recorded call and will not run at playback time. This property becomes
		/// true for all code run inside the scope of a RecorderManager.Lock.
		/// </remarks>
		public static bool IsInCall {
			get {
				return (recordNestLevel > 0);
			}
		}

		/// <summary>
		/// Ends the current recording session.
		/// </summary>
		public static void EndRecording() {
			if (action != RecorderState.Recording) throw new InvalidOperationException("Recorder is not currently recording.");
			if (OnEndRecoring != null) OnEndRecoring(recorder, EventArgs.Empty);
			recorder.EndRecording();
			MockService.ResetSession();
			currentRecordingName = null;
			action = RecorderState.None;
		}


		/// <summary>
		/// Generates a new instance name for an object of the given type.
		/// </summary>
		/// <param name="objectType">The type of object to get a new instance name.</param>
		/// <remarks>
		/// This call is only valid during recording sessions.
		/// </remarks>
		public static string GetNextInstanceName(Type objectType) {
			if (action != RecorderState.Recording) throw new InvalidOperationException("Recorder is not currently recording. This call is only valid during recording sessions.");
			return recorder.GetNextInstanceName(objectType);
		}

		/// <summary>
		/// Returns a playback session instance.
		/// </summary>
		/// <remarks>To be used in a using block.</remarks>
		public static IDisposable NewPlayBackSession(string recordingName, bool validating) {
			return new PlaybackSession(recordingName, validating);
		}

		/// <summary>
		/// Initiates a playback session.
		/// </summary>
		public static void BeginPlayBack(string recordingName) {
			if (recorder == null) throw new InvalidOperationException("A recorder needs to be registered to the recorder before starting a RecorderManager session.");
			if (action != RecorderState.None) throw new InvalidOperationException("Recorder is already busy playing or recording.");
			currentRecordingName = recordingName;
			MockService.ResetSession();
			recorder.BeginPlayBack(recordingName);
			action = RecorderState.PlayBack;
			if (OnBeginPlayback != null) OnBeginPlayback(recorder, EventArgs.Empty);
		}

		/// <summary>
		/// Plays back the given call.
		/// </summary>
		public static void PlayBackCall(MockableCall call) {
			recorder.PlayBackCall(call);
		}

		/// <summary>
		/// Validates the current playback session.
		/// </summary>
		public static void ValidatePlayBack() {
			recorder.ValidatePlayBack();
		}

		/// <summary>
		/// Ends the current playback session.
		/// </summary>
		public static void EndPlayBack() {
			if (action != RecorderState.PlayBack) throw new InvalidOperationException("Recorder is not currently playing.");
			if (OnEndPlayback != null) OnEndPlayback(recorder, EventArgs.Empty);
			recorder.EndPlayBack();
			MockService.ResetSession();
			currentRecordingName = null;
			action = RecorderState.None;
		}

		internal class PlaybackSession : IDisposable {

			private bool validating = false;

			public PlaybackSession(string recordingName, bool validating) {
				this.validating = validating;
				RecorderManager.BeginPlayBack(recordingName);
			}

			public void Dispose() {
				if (validating) RecorderManager.ValidatePlayBack();
				RecorderManager.EndPlayBack();
			}
		}

		internal class RecordingSession : IDisposable {

			public RecordingSession(string recordingName) {
				RecorderManager.BeginRecording(recordingName);
			}

			public void Dispose() {
				RecorderManager.EndRecording();
			}
		}

		/// <summary>
		/// Used within a using construct to lock out the RecorderManager from recording
		/// calls.
		/// </summary>
		public class Lock : IDisposable {
			private int rsl = 0;

			/// <summary>
			/// Creates a new lock.
			/// </summary>
			public Lock() { rsl = RecorderManager.recordNestLevel++; }

			/// <summary>
			/// Disposes the lock.
			/// </summary>
			public void Dispose() { RecorderManager.recordNestLevel = rsl; }
		}
	}
}
