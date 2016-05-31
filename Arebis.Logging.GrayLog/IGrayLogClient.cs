using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Logging.GrayLog
{
    public interface IGrayLogClient : IDisposable
    {
        /// <summary>
        /// Sents a message to GrayLog.
        /// </summary>
        /// <param name="shortMessage">Short message text (required).</param>
        /// <param name="fullMessage">Full message text.</param>
        /// <param name="data">Additional details object. Can be a plain object, a string, an enumerable or a dictionary.</param>
        /// <param name="ex">An exception to log data of.</param>
        void Send(string shortMessage, string fullMessage = null, object data = null, Exception ex = null);

        /// <summary>
        /// Convenience method to send an exception message to GrayLog.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="level">The level to log the exception at.</param>
        void Send(Exception ex, SyslogLevel level = SyslogLevel.Error);
    }
}
