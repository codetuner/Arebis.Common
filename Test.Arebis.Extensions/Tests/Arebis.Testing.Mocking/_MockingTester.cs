using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Mocking;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking
{
    [TestClass]
    public abstract class _MockingTester
    {
        #region Setup & Teardown

        private Support.TestingRecorder currentRecorder;

        [TestInitialize()]
        public virtual void TestSetup()
        {
            if (RecorderManager.IsPlaying) RecorderManager.EndPlayBack();
            if (RecorderManager.IsRecording) RecorderManager.EndRecording();
            this.currentRecorder = (Support.TestingRecorder)(RecorderManager.Recorder = new Support.TestingRecorder());
            global::Arebis.Testing.Mocking.Remoting.RemotingMockService.ResetGlobal();
            Sample.CurrencyServiceFactory.NextInstance = null;
        }

        [TestCleanup()]
        public virtual void TestTearDown()
        {
        }


        internal Support.TestingRecorder CurrentRecorder
        {
            get
            {
                return this.currentRecorder;
            }
        }

        #endregion
    }
}
