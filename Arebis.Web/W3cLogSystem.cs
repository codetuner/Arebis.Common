using Arebis.IO;
using Arebis.Threading;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;

namespace Arebis.Web
{
    public static class W3cLogSystem
    {
        private static readonly object StaticSyncRoot = new Object();

        private static volatile DedicatedWorkThread<string> LoggingThread;

        private static string Filename;

        private static TextWriter LogWriter;

        public static void Log(HttpContext context)
        {
            if (LoggingThread == null) InitializeFile(context);

            var sb = new StringBuilder();
            var request = context.Request;
            var requestTimeUtc = ((DateTime)context.Items["_W3cLog_Rt"]);
            var requestTimeEndUtc = Current.DateTime.UtcNow;
            var timeTaken = (int)((requestTimeEndUtc - requestTimeUtc).TotalMilliseconds);
            var servervars = request.ServerVariables;
            var reqheaders = request.Headers;
            var response = context.Response;

            // Write log record, server data:
            sb.Append(requestTimeUtc.ToString("yyyy-MM-dd HH:mm:ss ")); // Date and Time (UTC)
            sb.Append(LogFormatted(servervars["LOCAL_ADDR"])); // Local IP address
            sb.Append(' ');
            sb.Append(LogFormatted(servervars["SERVER_PROTOCOL"])); // ex. HTTP/1.0
            sb.Append(' ');
            sb.Append(LogFormatted(servervars["SERVER_PORT"])); // IP port on the server
            sb.Append(' ');
            sb.Append(LogFormatted(servervars["HTTPS"])); // 'on' or 'off' (HTTPS or HTTP)
            sb.Append(' ');
            sb.Append(LogFormatted(servervars["HTTP_HOST"])); // Hostname of this site
            sb.Append(' ');

            // Write log record, request data:
            sb.Append(LogFormatted(servervars["REQUEST_METHOD"])); // Request method: GET, POST, HEAD,...
            sb.Append(' ');
            sb.Append(LogFormatted(servervars["SCRIPT_NAME"])); // Name and path of the script/document
            sb.Append(' ');
            sb.Append(LogFormatted(servervars["QUERY_STRING"])); // Query parameters
            sb.Append(' ');
            sb.Append(LogFormatted(servervars["HTTP_USER_AGENT"])); // User-agent (brower) signature
            sb.Append(' ');
            sb.Append(LogFormatted(servervars["HTTP_ACCEPT_LANGUAGE"])); // User-agent (browser) languages
            sb.Append(' ');
            sb.Append(LogFormatted(servervars["HTTP_REFERER"])); // Referer URL
            sb.Append(' ');

            // Write log record, user data:
            sb.Append(LogFormatted(servervars["REMOTE_ADDR"])); // IP address of the client
            sb.Append(' ');
            sb.Append(LogFormatted(servervars["REMOTE_USER"])); // 'Username' of a logged-in user
            sb.Append(' ');
            //sb.Append(LogFormatted(request.Cookies.Get("ASP.NET_SessionId")));
            sb.Append(LogFormatted((context.Session != null) ? context.Session.SessionID : null)); // ASP.NET Session ID
            sb.Append(' ');

            // Write log record, status and metrics data:
            sb.Append(response.StatusCode); // HTTP status code
            sb.Append(' ');
            sb.Append(response.SubStatusCode); // HTTP sub status code
            sb.Append(' ');
            sb.Append(timeTaken); // Call duration in milliseconds
            sb.Append(' ');
            sb.Append(request.TotalBytes + servervars["ALL_RAW"].Length); // Bytes sent to server (client-to-server); headers and request content
            sb.Append(' ');
            sb.Append(((MeteringStream)context.Items["_W3cLog_Os"]).BytesWritten); // Bytes sent to client (server-to-client); only resopnse content

            LoggingThread.AddWork(sb.ToString());
        }

        public static void Reset()
        {
            var t = LoggingThread;

            LoggingThread = null;

            if (t != null)
            {
                t.AddWork("#Closing.");
                t.Join();
            }
        }

        private static void InitializeFile(HttpContext context)
        {
            lock (StaticSyncRoot)
            {
                if (LoggingThread == null)
                {
                    // Get time:
                    var nowUtc = Current.DateTime.UtcNow;

                    // Create log folder:
                    var path = context.Server.MapPath(ConfigurationManager.AppSettings["W3cLogPath"] ?? "~/App_Data/Logs");
                    Directory.CreateDirectory(path);

                    // Get filename:
                    if (Filename == null)
                    {
                        Filename = Path.Combine(path, String.Format("{0:yyyyMMdd-HHmmss}-{1}-{2}-{3}.log", nowUtc, Environment.MachineName, context.Request.ServerVariables["INSTANCE_ID"], context.Request.ServerVariables["SERVER_NAME"]));
                    }

                    // Create logging thread:
                    LoggingThread = new DedicatedWorkThread<string>(
                        WhenLogHandlerDo,
                        10000,
                        "AsyncLoggingThread");

                    // Connect event handlers:
                    LoggingThread.Reactivate += WhenLogHandlerReactivate;
                    LoggingThread.Idle += WhenLogHandlerIdle;
                    LoggingThread.Failed += WhenLogHandlerFailed;
                    AppDomain.CurrentDomain.DomainUnload += WhenAppDomainUnload;
                }
            }
        }

        static void WhenLogHandlerReactivate(object sender, EventArgs e)
        {
            var exists = File.Exists(Filename);
            LogWriter = new StreamWriter(new FileStream(Filename, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);
            if (!exists)
            {
                // Write file header:
                var nowUtc = Current.DateTime.UtcNow;
                LogWriter.WriteLine("#Software: Arebis W3cLogModule 1.0");
                LogWriter.WriteLine("#Version: 1.0");
                LogWriter.Write("#Date: ");
                LogWriter.WriteLine(nowUtc.ToString("yyyy-MM-dd HH:mm:ss"));
                LogWriter.Write("#MachineDate: ");
                LogWriter.WriteLine(nowUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                LogWriter.Write("#MachineName: ");
                LogWriter.WriteLine(System.Environment.MachineName);
                LogWriter.Write("#OSVersion: ");
                LogWriter.WriteLine(System.Environment.OSVersion);
                LogWriter.Write("#ProcessorCount: ");
                LogWriter.WriteLine(System.Environment.ProcessorCount);
                LogWriter.WriteLine("#");
                LogWriter.WriteLine("#Fields: date time s-ip cs-version s-port x-cs-https cs-host cs-method cs-uri-stem cs-uri-query cs(User-Agent) x-cs-accept-language cs(Referer) c-ip cs-username x-asp-session sc-status sc-substatus time-taken cs-bytes sc-bytes");
            }
        }

        private static void WhenLogHandlerDo(string record)
        {
            LogWriter.WriteLine(record);
        }

        private static void WhenLogHandlerFailed(object sender, WorkItemExceptionEventArgs<string> e)
        {
            Debug.WriteLine(e.WorkItem);
        }

        private static void WhenLogHandlerIdle(object sender, EventArgs e)
        {
            LogWriter.Flush();
            LogWriter.Dispose();
        }

        private static void WhenAppDomainUnload(object sender, EventArgs e)
        {
            Reset();
        }

        private static string LogFormatted(string str)
        {
            if (String.IsNullOrEmpty(str))
                return "-";
            else
                return str.Replace(" ", "+");
        }

        private static string LogFormatted(HttpCookie cookie)
        {
            if (cookie == null)
                return LogFormatted((string)null);
            else
                return LogFormatted(cookie.Value);
        }
    }
}
