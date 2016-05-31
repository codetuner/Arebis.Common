using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Tasks
{
    public class SimpleConsoleTaskHost : SimpleTaskHost
    {
        /// <summary>
        /// Creates a new SimpleConsoleTaskHost.
        /// </summary>
        /// <param name="abortByCancelKey">Whether the console cancel key (Ctrl+Break) should request task gracefull abortion.</param>
        public SimpleConsoleTaskHost(bool abortByCancelKey)
        {
            if (abortByCancelKey) Console.CancelKeyPress += ConsoleCancelKeyHandler;
        }

        void ConsoleCancelKeyHandler(object sender, ConsoleCancelEventArgs e)
        {
            RequestAbort();
            e.Cancel = true;
        }

        public override TextReader In
        {
            get
            {
                return Console.In;
            }
        }

        public override TextWriter Out
        {
            get
            {
                return Console.Out;
            }
        }

        public override TextWriter Error
        {
            get
            {
                return Console.Error;
            }
        }

        public override void Dispose()
        {
            Console.CancelKeyPress -= ConsoleCancelKeyHandler;

            base.Dispose();
        }
    }
}
