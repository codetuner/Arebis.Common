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
        void Send(string shortMessage, string fullMessage = null, object data = null);

        /// <summary>
        /// Convenience method to send an exception message to GrayLog.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        void Send(Exception ex);
    }
}
