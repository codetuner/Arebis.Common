using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Logging.GrayLog
{
    /// <summary>
    /// A GrayLog client using the HTTP(S) protocol.
    /// </summary>
    public class GrayLogHttpClient : GrayLogClient
    {
        /// <summary>
        /// Creates a new GrayLogHttpClient using the "GrayLogFacility", "GrayLogHost" and optionally "GrayLogHttpPort" and "GrayLogHttpSecure" (true or false) AppSettings.
        /// </summary>
        public GrayLogHttpClient()
            : this(ConfigurationManager.AppSettings["GrayLogFacility"], ConfigurationManager.AppSettings["GrayLogHost"], Int32.Parse(ConfigurationManager.AppSettings["GrayLogHttpPort"] ?? "12201"), Boolean.Parse(ConfigurationManager.AppSettings["GrayLogHttpSecure"] ?? "false"))
        { }

        /// <summary>
        /// Creates a new GrayLogHttpClient.
        /// </summary>
        /// <param name="facility">Facility to set on all sent messages.</param>
        /// <param name="host">GrayLog host name.</param>
        /// <param name="port">GrayLog HTTP port.</param>
        /// <param name="useSsl">Whether to use SSL (not supported by GrayLog at this time).</param>
        public GrayLogHttpClient(string facility, string host, int port = 12201, bool useSsl = false)
            : this(facility, new Uri((useSsl ? "https://" : "http://") + host + ":" + port + "/gelf"))
        { }

        /// <summary>
        /// Creates a new GrayLogHttpClient.
        /// </summary>
        /// <param name="facility">Facility to set on all sent messages.</param>
        /// <param name="uri">GrayLog URL to send GELF messages to.</param>
        protected GrayLogHttpClient(string facility, Uri uri)
            : base(facility)
        {
            this.Uri = uri;
        }

        /// <summary>
        /// GrayLog URL to send GELF messages to.
        /// </summary>
        public Uri Uri { get; private set; }

        protected override void InternallySendMessage(byte[] messageBody)
        {
            // Apply compression:
            var compressed = false;
            if (this.CompressionTreshold != -1 && messageBody.Length > this.CompressionTreshold)
            {
                compressed = true;
                messageBody = this.Compress(messageBody, CompressionLevel.Optimal);
            }

            var req = (HttpWebRequest)WebRequest.Create(this.Uri);
            req.ServicePoint.Expect100Continue = false;
            req.Method = "POST";
            req.ContentType = "application/json; charset=UTF-8";
            req.ContentLength = messageBody.Length;
            req.Expect = "";
            if (compressed) req.Headers.Add(HttpRequestHeader.ContentEncoding, "gzip");
            using (var reqs = req.GetRequestStream())
            {
                reqs.Write(messageBody, 0, messageBody.Length);
            }

            req.GetResponse();
        }

        public override void Dispose()
        { }
    }
}
