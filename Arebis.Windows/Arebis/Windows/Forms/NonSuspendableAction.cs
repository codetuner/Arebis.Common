using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arebis.WinApi;

namespace Arebis.Windows.Forms
{
    /// <summary>
    /// Used as a using-block resource, marks a code section during
    /// which system suspension should be disabled.
    /// </summary>
    public class NonSuspendableAction : IDisposable
    {
        private EXECUTION_STATE previousState;

        public NonSuspendableAction()
        {
            // Forces the system to be in the working state until next call that uses ES_CONTINUOUS:
            previousState = Kernel32.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
        }

        public void Dispose()
        {
            // Clear EXECUTION_STATE flags to disable away mode and allow the system to idle to sleep normally:
            Kernel32.SetThreadExecutionState(previousState | EXECUTION_STATE.ES_CONTINUOUS);
        }
    }
}
