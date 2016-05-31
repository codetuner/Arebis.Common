using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Logging
{
    /// <summary>
    /// Serializes log records on a stream.
    /// </summary>
    public interface ILogFormatter
    {
        /// <summary>
        /// Serializes the given log record on the given stream.
        /// </summary>
        void WriteLog(Stream stream, LogRecord record);
    }
}
