using System;

namespace Arebis.Pdf.Common
{
    [Serializable]
    public class PdfBinaryStream : PdfStream
    {
        private string filter;

        public PdfBinaryStream(string filter, byte[] content)
        {
            this.filter = filter;
            this.Content = content;
        }

        public override int Length
        {
            get { return Content.Length; }
        }

        public override string Filter
        {
            get { return filter; }
        }

        public byte[] Content { get; private set; }
    }
}
