using System;
using System.Collections.Generic;

namespace Arebis.Pdf.Common
{
    [Serializable]
    public class PdfObject
    {
        public PdfObject()
        {
            this.Data = new Dictionary<string, object>();
        }

        public Dictionary<String, Object> Data { get; set; }

        public PdfStream Stream { get; set; }
    }
}
