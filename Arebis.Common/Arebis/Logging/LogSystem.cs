using Arebis.Threading;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Arebis.Logging
{
    public static class LogSystem
    {
        private static readonly DedicatedWorkThread<LogRecord> _loggingThread;

        private static volatile bool _isAsynchroneous;

        private static readonly object _staticSyncRoot = new Object();

        static LogSystem()
        {
            LoggingKeepAliveTime = TimeSpan.Parse(ConfigurationManager.AppSettings["LoggingKeepAliveTime"] ?? "0");

            LoggingEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["LoggingEnabled"] ?? "False");

            IsAsynchroneous = Convert.ToBoolean(ConfigurationManager.AppSettings["LoggingAsynchroneous"] ?? "False");

            LogWriter = (ILogWriter)Activator.CreateInstance(Type.GetType(ConfigurationManager.AppSettings["LogWriterType"] /*?? typeof(DefaultLogWriter).AssemblyQualifiedName*/));

            _loggingThread = new DedicatedWorkThread<LogRecord>(
                WhenLogHandlerDo,
                (int)LoggingKeepAliveTime.TotalMilliseconds,
                "AsyncLoggingThread");
            _loggingThread.Idle += WhenLogHandlerIdle;
            _loggingThread.Failed += WhenLogHandlerFailed;

            AppDomain.CurrentDomain.DomainUnload += WhenAppDomainUnload;
        }

        /// <summary>
        /// Whether logging is enabled.
        /// Set by "LoggingEnabled" AppSetting, default "False".
        /// </summary>
        public static bool LoggingEnabled { get; set; }

        /// <summary>
        /// ILogWriter instance to write logging to.
        /// Set by "LogWriterType" AppSetting.
        /// </summary>
        public static ILogWriter LogWriter { get; set; }

        /// <summary>
        /// When logging asynchroneous, time to keep logging thread alive before turning to idle state.
        /// Set by "LoggingKeepAliveTime" AppSetting. Default "00:00:00".
        /// </summary>
        public static TimeSpan LoggingKeepAliveTime { get; private set; }

        /// <summary>
        /// Whether logging is asynchroneous.
        /// Set by "LoggingAsynchroneous" AppSetting, default "False".
        /// </summary>
        public static bool IsAsynchroneous
        {
            get
            {
                return _isAsynchroneous;
            }
            set
            {
                lock (_staticSyncRoot)
                {
                    if (value == true)
                    {
                        _isAsynchroneous = true;
                    }
                    else if (_isAsynchroneous == true)
                    {
                        _loggingThread.Join();
                        _isAsynchroneous = false;
                    }
                }
            }
        }

        //... public Log methods

        private static void Log(LogRecord loggingRecord)
        {
            try
            {
                lock (_staticSyncRoot)
                {
                    if (LoggingEnabled)
                    {
                        if (IsAsynchroneous)
                        {
                            _loggingThread.AddWork(loggingRecord);
                        }
                        else
                        {
                            if (LogWriter != null)
                            {
                                LogWriter.Write(loggingRecord);
                                LogWriter.Flush();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FallbackLogging(loggingRecord, ex);
            }
        }

        public static void FallbackLogging(LogRecord loggingRecord, Exception ex)
        {
            try
            {
                if (ex != null)
                {
                    Debug.WriteLine("Logging failure : " + ex.ToString());
                }
            }
            catch
            { }
        }

        public static void Close()
        {
            LoggingEnabled = false;
            _loggingThread.Join();
        }

        public static void Close(TimeSpan timeOut)
        {
            LoggingEnabled = false;
            _loggingThread.Join(timeOut);
        }

        private static void WhenLogHandlerDo(LogRecord record)
        {
            if (LogWriter != null)
                LogWriter.Write(record);
        }

        private static void WhenLogHandlerIdle(object sender, EventArgs e)
        {
            if (LogWriter != null)
                LogWriter.Flush();
        }

        private static void WhenLogHandlerFailed(object sender, WorkItemExceptionEventArgs<LogRecord> e)
        {
            FallbackLogging(e.WorkItem, e.Exception);
        }

        private static void WhenAppDomainUnload(object sender, EventArgs e)
        {
            // Close the LogSystem forcing a Join with the logging thread
            // ensuring all queued log records are written before unloading
            // the AppDomain:
            Close();
        }
    }
}
