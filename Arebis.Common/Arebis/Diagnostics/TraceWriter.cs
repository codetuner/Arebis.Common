using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Diagnostics
{
    /// <summary>
    /// A TextWriter implementation that writes to the trace listeners in the System.Diagnostics.Trace.Listeners collection.
    /// </summary>
    public class TraceWriter : System.IO.TextWriter
    {
        public override void Write(object value)
        {
            System.Diagnostics.Trace.Write(value);
        }

        public override void Write(string message)
        {
            System.Diagnostics.Trace.Write(message);
        }

        public override void WriteLine(object value)
        {
            System.Diagnostics.Trace.WriteLine(value);
        }

        public override void WriteLine(string message)
        {
            System.Diagnostics.Trace.WriteLine(message);
        }

        public override void Write(char value)
        {
            System.Diagnostics.Trace.Write(value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
