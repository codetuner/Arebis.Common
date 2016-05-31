using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Arebis.Windows.Forms
{
    /// <summary>
    /// Used as a using-block resource, marks a code section during
    /// which a wait mouse cursor is shown for the given window.
    /// </summary>
    public class WaitCursor : IDisposable
    {
        private Control window;
        private Cursor previousCursor;

        public WaitCursor(Control window)
        {
            this.window = window;
            this.previousCursor = this.window.Cursor;
            this.window.Cursor = Cursors.WaitCursor;            
        }

        public void Dispose()
        {
            this.window.Cursor = this.previousCursor;
        }
    }
}
