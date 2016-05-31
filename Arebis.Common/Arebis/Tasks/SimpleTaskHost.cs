using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Tasks
{
    public class SimpleTaskHost : ISimpleTaskHost
    {
        private Stopwatch stopwatch;
        private volatile bool isAbortRequested;
        private volatile bool isAborting;
        private TextReader @in;
        private TextWriter @out;
        private TextWriter error;

        public event ProgressEventHandler Progress;

        public SimpleTaskHost()
        {
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
        }

        public virtual TextReader In
        {
            get { return (@in = @in ?? new StreamReader(new MemoryStream())); }
        }

        public virtual TextWriter Out
        {
            get { return (@out = @out ?? new StreamWriter(new MemoryStream())); }
        }

        public virtual TextWriter Error
        {
            get { return (error = error ?? new StreamWriter(new MemoryStream())); }
        }

        public void RequestAbort()
        {
            this.isAbortRequested = true;
        }

        public virtual bool IsAbortRequested
        {
            get { return this.isAbortRequested; }
        }

        public virtual bool IsAborting
        {
            get { return this.isAborting; }
        }

        public virtual void Aborting()
        {
            this.isAborting = true;
        }

        public virtual void ReportProgress(double progressFraction, string progressMessage = null)
        {
            OnProgress(new ProgressEventArgs() { ProgressFraction = progressFraction, ProgressMessage = progressMessage });
        }

        public virtual TimeSpan TimeElapsed
        {
            get { return this.stopwatch.Elapsed; }
        }

        public virtual void Dispose()
        {
            if (@in != null) @in.Dispose();
            if (@out != null) @out.Dispose();
            if (error != null) error.Dispose();
        }

        public object Result { get; set; }

        protected virtual void OnProgress(ProgressEventArgs e)
        {
            if (Progress != null) Progress(this, e);
        }
    }
}
