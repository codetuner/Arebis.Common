using System;
using System.Text;

namespace Arebis.Pdf.Common
{
    [Serializable]
    public class PdfTextStream : PdfStream
    {
        public PdfTextStream()
        {
            this.Content = new StringBuilder();
        }

        public StringBuilder Content { get; set; }

        public override int Length
        {
            get { return Content.Length; }
        }

        public override string Filter
        {
            get { return null; }
        }
    }
}
