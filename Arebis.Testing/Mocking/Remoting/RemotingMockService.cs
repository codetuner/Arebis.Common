using System;
using System.Collections;
using System.Text.RegularExpressions;
using Arebis.Mocking;

namespace Arebis.Testing.Mocking.Remoting
{

	/// <summary>
	/// Service class for the Remoting extension of the mocking framework.
	/// Provides static methods to control the remoting mocking.
	/// </summary>
	public sealed class RemotingMockService {

		private static Hashtable locallyMockingUriPatterns = new Hashtable();
		private static Hashtable globallyMockingUriPatterns = new Hashtable();

		private RemotingMockService() {
			throw new InvalidOperationException("RemotingMockService is not to be instantiated.");
		}

		static RemotingMockService() {
			RecorderManager.OnBeginPlayback += new EventHandler(OnRecorderStateChange);
			RecorderManager.OnBeginRecording += new EventHandler(OnRecorderStateChange);
			RecorderManager.OnEndPlayback += new EventHandler(OnRecorderStateChange);
			RecorderManager.OnEndRecoring += new EventHandler(OnRecorderStateChange);
		}

		/// <summary>
		/// Adds an URI pattern for which mocking should be enabled.
		/// </summary>
		/// <param name="uripattern">The pattern for which matching URI's should be mocked.</param>
		/// <remarks>
		/// The pattern supports '*' as wildcard for multiple characters and '?' as wildcard for one character.
		/// This method must be called inside a recording or playback session.
		/// The provided URI pattern will only be mocked in the currently running session.
		/// </remarks>
		public static void AddMockingUri(string uripattern) {
			Regex matcherRegex = MatcherRegex(uripattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			if (RecorderManager.IsPlaying || RecorderManager.IsRecording)
				locallyMockingUriPatterns[uripattern] = matcherRegex;
			else
				throw new InvalidOperationException("Recorder must be in a playing or recording session.");
		}

		/// <summary>
		/// Removes an URI pattern for which mocking should be enabled.
		/// </summary>
		/// <param name="uripattern">The pattern for which matching URI's should not be mocked anymore.</param>
		/// <remarks>
		/// This method must be called inside a recording or playback session.
		/// This method removes patterns previously added with AddMockingUri.
		/// </remarks>
		public static void RemoveMockingUri(string uripattern) {
			if (RecorderManager.IsPlaying || RecorderManager.IsRecording)
				locallyMockingUriPatterns.Remove(uripattern);
			else
				throw new InvalidOperationException("Recorder must be in a playing or recording session.");
		}

		/// <summary>
		/// Adds an URI pattern for which mocking should be enabled.
		/// </summary>
		/// <param name="uripattern">The pattern for which matching URI's should be mocked.</param>
		/// <remarks>
		/// The pattern supports '*' as wildcard for multiple characters and '?' as wildcard for one character.
		/// This method must be called outside a recording or playback session.
		/// The provided URI pattern will be mocked during all future session.
		/// </remarks>
		public static void AddGlobalMockingUri(string uripattern) {
			Regex matcherRegex = MatcherRegex(uripattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			if (RecorderManager.IsPlaying || RecorderManager.IsRecording)
				throw new InvalidOperationException("Recorder must not be in a playing or recording session.");
			else
				globallyMockingUriPatterns[uripattern] = matcherRegex;
		}

		/// <summary>
		/// Removes an URI pattern for which mocking should be enabled.
		/// </summary>
		/// <param name="uripattern">The pattern for which matching URI's should not be mocked anymore.</param>
		/// <remarks>
		/// This method removes patterns previously added with AddGlobalMockingUri.
		/// This method must be called outside a recording or playback session.
		/// </remarks>
		public static void RemoveGlobalMockingUri(string uripattern) {
			if (RecorderManager.IsPlaying || RecorderManager.IsRecording)
				throw new InvalidOperationException("Recorder must not be in a playing or recording session.");
			else
				globallyMockingUriPatterns.Remove(uripattern);
		}
		
		/// <summary>
		/// Resets the list of global mocking URI patterns.
		/// </summary>
		/// <remarks>
		/// This method must be called outside a recording or playback session.
		/// </remarks>
		public static void ResetGlobal() {
			if (RecorderManager.IsPlaying || RecorderManager.IsRecording)
				throw new InvalidOperationException("Recorder must not be in a playing or recording session.");
			else
				globallyMockingUriPatterns = new Hashtable();
		}

		/// <summary>
		/// Tests whether the given URI is to be mocked.
		/// </summary>
		public static bool IsUriToMock(string uri) {
			foreach(Regex matcher in globallyMockingUriPatterns.Values)
				if (matcher.IsMatch(uri)) return true;
			foreach(Regex matcher in locallyMockingUriPatterns.Values)
				if (matcher.IsMatch(uri)) return true;
			return false;
		}

		/// <summary>
		/// Handles the state change events of the recorder.
		/// </summary>
		private static void OnRecorderStateChange(object sender, EventArgs e) {
			// Clear the locallyMockingUriPatterns:
			locallyMockingUriPatterns = new Hashtable();
		}

		/// <summary>
		/// Returns a RegEx to match the given pattern with wildcards * and ?.
		/// </summary>
		private static Regex MatcherRegex(string pattern, RegexOptions options) {
			pattern = Regex.Escape(pattern);
			pattern = "^" + pattern.Replace("\\*", ".*").Replace("\\?", ".") + "$";
			return new Regex(pattern, options);
		}
	}
}
