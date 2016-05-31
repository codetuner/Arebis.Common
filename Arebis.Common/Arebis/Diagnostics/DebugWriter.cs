using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Diagnostics
{
    /// <summary>
    /// A TextWriter implementation that writes to the trace listeners in the System.Diagnostics.Debug.Listeners collection.
    /// </summary>
    public class DebugWriter : System.IO.TextWriter
    {
        public override void Write(object value)
        {
            System.Diagnostics.Debug.Write(value);
        }

        public override void Write(string message)
        {
            System.Diagnostics.Debug.Write(message);
        }

        public override void WriteLine(object value)
        {
            System.Diagnostics.Debug.WriteLine(value);
        }

        public override void WriteLine(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public override void WriteLine(string format, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(format, args);
        }

        public override void Write(char value)
        {
            System.Diagnostics.Debug.Write(value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
