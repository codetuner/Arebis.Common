using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Arebis.Windows.Forms
{
    public partial class ClipboardListener : Component
    {
        private ListenerForm listenerForm;

        private bool enabled = false;
        private int skipNext = 0;

        public event EventHandler ItemCopied;

        [Browsable(true), DefaultValue(false)]
        public bool Enabled
        {
            get { return this.enabled; }
            set 
            { 
                this.enabled = value;
                if (this.listenerForm == null)
                {
                    this.listenerForm = new ListenerForm();
                    this.listenerForm.DataCopied += new EventHandler(listenerForm_DataCopied);
                }
                if (this.enabled)
                    this.listenerForm.StartListening();
                else
                    this.listenerForm.StopListening();
            }
        }

        public void SkipNext()
        {
            if (this.Enabled)
            {
                this.skipNext = 2;
            }
        }

        void listenerForm_DataCopied(object sender, EventArgs e)
        {
            if (skipNext > 0)
            {
                skipNext--;
            }
            else 
            {
                if (this.ItemCopied != null) this.ItemCopied(this, EventArgs.Empty);
            }
        }

        private class ListenerForm : Form
        {
            private bool listening = false;
            private IntPtr _ClipboardViewerNext;

            internal event EventHandler DataCopied;

            internal ListenerForm()
            {
                InitializeComponent();
            }

            private void InitializeComponent()
            {
                this.Visible = false;
                this.FormClosed += new FormClosedEventHandler(ListenerForm_FormClosed);
            }

            private void ListenerForm_FormClosed(object sender, FormClosedEventArgs e)
            {
                StopListening();
            }

            protected override void WndProc(ref Message m)
            {
                switch ((System.Win32.Msgs)m.Msg)
                {
                    case System.Win32.Msgs.WM_CHANGECBCHAIN: // The chain of clipboard viewers is changing:
                        // If the next window is closing, repair the chain. 
                        if (m.WParam == _ClipboardViewerNext)
                            _ClipboardViewerNext = m.LParam;
                        // Otherwise, pass the message to the next link. 
                        else
                            System.Win32.User32.SendMessage(_ClipboardViewerNext, m.Msg, m.WParam, m.LParam);
                        break;
                    case System.Win32.Msgs.WM_DRAWCLIPBOARD: // clipboard contents changed. 
                        // Clipboard has changed, update the value:
                        if (this.DataCopied != null) this.DataCopied(this, EventArgs.Empty);
                        // Pass the message to the next window in clipboard viewer chain. 
                        System.Win32.User32.SendMessage(_ClipboardViewerNext, m.Msg, m.WParam, m.LParam);
                        break;
                    case System.Win32.Msgs.WM_POWERBROADCAST: // System enters/leaves suspend or other Power action happens:
                        //Reconnect();
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }
            }

            public void Reconnect()
            {
                if (listening == true)
                {
                    this.StopListening();
                    this.StartListening();
                }
            }

            public void StartListening()
            {
                if (listening == false)
                {
                    _ClipboardViewerNext = System.Win32.User32.SetClipboardViewer(this.Handle);
                    listening = true;
                }
            }

            public void StopListening()
            {
                if (listening == true)
                {
                    System.Win32.User32.ChangeClipboardChain(this.Handle, _ClipboardViewerNext);
                    listening = false;
                }
            }
        }

    }
}
