using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Mocking;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking
{

	[TestClass()]
	public class RecorderTests : _MockingTester
	{

		[TestMethod()]
		public void PlaybackTest()
		{
			Support.RecorderListener listener = new Support.RecorderListener();
			RecorderManager.BeginPlayBack("test");
			using (new RecorderManager.Lock())
			{
			}
			RecorderManager.EndPlayBack();
			Assert.AreEqual(1, listener.BeginPlaybackCounter);
			Assert.AreEqual(1, listener.EndPlaybackCounter);
		}

		[TestMethod()]
		public void PlayBackBeginEnd00Test()
		{
			Assert.IsFalse(RecorderManager.IsPlaying);
			RecorderManager.BeginPlayBack("test");
			Assert.IsTrue(RecorderManager.IsPlaying);
			RecorderManager.EndPlayBack();
			Assert.IsFalse(RecorderManager.IsPlaying);
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException), "Recorder is already busy playing or recording.")]
		public void PlayBackBeginEnd01Test()
		{
			RecorderManager.BeginPlayBack("test");
			RecorderManager.BeginPlayBack("test");
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException), "Recorder is not currently playing.")]
		public void PlayBackBeginEnd02Test()
		{
			RecorderManager.EndPlayBack();
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException), "Recorder is not currently playing.")]
		public void PlayBackBeginEnd03Test()
		{
			RecorderManager.BeginPlayBack("test");
			RecorderManager.EndPlayBack();
			RecorderManager.EndPlayBack();
		}

		[TestMethod()]
		public void RecordingTest()
		{
			Support.RecorderListener listener = new Support.RecorderListener();
			RecorderManager.BeginRecording("test");
			using (new RecorderManager.Lock())
			{
				RecorderManager.RecordCall(null);
			}
			using (new RecorderManager.Lock())
			{
				RecorderManager.RecordCall(null);
			}
			RecorderManager.EndRecording();
			Assert.AreEqual(1, listener.BeginRecordingCounter);
			Assert.AreEqual(1, listener.EndRecordingCounter);
			Assert.AreEqual(2, CurrentRecorder.Count);
		}

		[TestMethod()]
		public void RecordingBeginEnd00Test()
		{
			RecorderManager.BeginRecording("test");
			Assert.IsTrue(RecorderManager.IsRecording);
			RecorderManager.EndRecording();
			Assert.IsFalse(RecorderManager.IsRecording);
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException), "Recorder is already busy playing or recording.")]
		public void RecordingBeginEnd01Test()
		{
			RecorderManager.BeginRecording("test");
			RecorderManager.BeginRecording("test");
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException), "Recorder is not currently recording.")]
		public void RecordingBeginEnd02Test()
		{
			RecorderManager.EndRecording();
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException), "Recorder is not currently recording.")]
		public void RecordingBeginEnd03Test()
		{
			RecorderManager.BeginRecording("test");
			RecorderManager.EndRecording();
			RecorderManager.EndRecording();
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException), "RecorderManager should be locked before recording a call.")]
		public void RecordingNeedsLockTest()
		{
			RecorderManager.BeginRecording("test");
			RecorderManager.RecordCall(null);
			RecorderManager.EndRecording();
		}

		[TestMethod()]
		public void RecorderSwitch00Test()
		{
			RecorderManager.Recorder = null;
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException), "Cannot switch recorder while busy playing.")]
		public void RecorderSwitch01Test()
		{
			RecorderManager.BeginPlayBack("test");
			RecorderManager.Recorder = null;
		}
	}
}
