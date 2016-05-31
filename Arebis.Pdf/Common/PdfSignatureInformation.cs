using System;
using System.Security.Cryptography.X509Certificates;

namespace Arebis.Pdf.Common
{
    public class PdfSignatureInformation
    {
        public X509Certificate2 Certificate{ get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public string Name { get; set; }

        public string ContactInfo { get; set; }

        public string Location { get; set; }

        public string Reason { get; set; }
    }
}
