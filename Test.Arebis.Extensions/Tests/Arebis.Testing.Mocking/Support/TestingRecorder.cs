using System;
using System.Collections;
using Arebis.Mocking;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking.Support
{
	internal class TestingRecorder : IRecorder
	{

		private string recordingName;
		private ArrayList records;
		private bool validated;
		private bool ended;
		private int pointer;
		private int mockInstanceId;

		#region Accessors

		public string RecordingName
		{
			get { return recordingName; }
		}

		public ArrayList Records
		{
			get { return records; }
		}

		public bool Validated
		{
			get { return validated; }
		}

		public bool Ended
		{
			get { return ended; }
		}

		public int Count
		{
			get { return this.records.Count; }
		}

		public MockableCall this[int index]
		{
			get { return (MockableCall)this.records[index]; }
		}

		#endregion

		#region IRecorder Members

		public void BeginRecording(string recordingName)
		{
			this.recordingName = recordingName;
			this.records = new ArrayList();
			this.ended = false;
			this.mockInstanceId = 0;
		}

		public void RecordCall(MockableCall call)
		{
			this.records.Add(call);
		}

		public void EndRecording()
		{
			this.ended = true;
		}

		public string GetNextInstanceName(Type objectType)
		{
			return String.Format("{0:4}", ++this.mockInstanceId);
		}

		public void BeginPlayBack(string recordingName)
		{
			this.ended = false;
			this.pointer = 0;
		}

		public void PlayBackCall(MockableCall actualCall)
		{
			MockableCall expectedCall = (MockableCall)this.records[this.pointer++];
			Assert.AreEqual(expectedCall.MethodSignature, actualCall.MethodSignature);
			actualCall.SetResult(expectedCall);
		}

		public void ValidatePlayBack()
		{
			Assert.AreEqual(pointer, this.records.Count, "Played scenario failed validation: not all calls have been replayed.");
			this.validated = true;
		}

		public void EndPlayBack()
		{
			this.ended = true;
		}

		#endregion

	}

}
