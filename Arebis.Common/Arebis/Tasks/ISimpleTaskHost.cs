using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Tasks
{
    public interface ISimpleTaskHost : IDisposable
    {
        TextReader In { get; }

        TextWriter Out { get; }

        TextWriter Error { get; }

        bool IsAbortRequested { get; }

        bool IsAborting { get; }

        void Aborting();

        void ReportProgress(double progressFraction, string progressMessage = null);

        TimeSpan TimeElapsed { get; }

        object Result { get; set; }
    }
}
