using System;
using Arebis.Mocking;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking.Support
{
	internal class RecorderListener
	{
		int beginPlaybackCounter, endPlaybackCounter, beginRecordingCounter, endRecordingCounter;

		public RecorderListener()
		{
			RecorderManager.OnBeginPlayback += new EventHandler(BeginPlayback);
			RecorderManager.OnEndPlayback += new EventHandler(EndPlayback);
			RecorderManager.OnBeginRecording += new EventHandler(BeginRecording);
			RecorderManager.OnEndRecoring += new EventHandler(EndRecording);
		}

		public int BeginPlaybackCounter
		{
			get
			{
				return this.beginPlaybackCounter;
			}
		}

		public int EndPlaybackCounter
		{
			get
			{
				return this.endPlaybackCounter;
			}
		}

		public int BeginRecordingCounter
		{
			get
			{
				return this.beginRecordingCounter;
			}
		}

		public int EndRecordingCounter
		{
			get
			{
				return this.endRecordingCounter;
			}
		}

		public void BeginPlayback(object source, EventArgs e)
		{
			this.beginPlaybackCounter++;
		}

		public void EndPlayback(object source, EventArgs e)
		{
			this.endPlaybackCounter++;
		}

		public void BeginRecording(object source, EventArgs e)
		{
			this.beginRecordingCounter++;
		}

		public void EndRecording(object source, EventArgs e)
		{
			this.endRecordingCounter++;
		}
	}
}
