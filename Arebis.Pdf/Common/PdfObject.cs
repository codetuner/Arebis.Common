using Arebis.Pdf.Writing;
using System;
using System.Collections.Generic;

namespace Arebis.Pdf.Common
{
    [Serializable]
    public class PdfObject
    {
        internal PdfObject(PdfDocumentWriter documentWriter)
        {
            this.DocumentWriter = documentWriter;
            this.Data = new Dictionary<string, object>();
        }

        [Obsolete("Replaced by the PdfDocumentWriter.CreatePdfObject() method.")]
        public PdfObject()
        {
            this.Data = new Dictionary<string, object>();
        }

        protected PdfDocumentWriter DocumentWriter { get; private set; }

        public Dictionary<String, Object> Data { get; set; }

        public PdfStream Stream { get; set; }
    }
}
