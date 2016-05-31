using Arebis.Pdf.Common;
using System;

namespace Arebis.Pdf.Writing
{
    [Serializable]
    public class PdfDocumentOptions
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string Subject { get; set; }

        public string Keywords { get; set; }

        public PdfStreamFilter TextFilter { get; set; }
    }
}
