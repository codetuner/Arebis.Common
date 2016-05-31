using System;

namespace Arebis.Pdf.Common
{
    [Serializable]
    public abstract class PdfStream
    {
        public abstract int Length { get; }

        public abstract string Filter { get; }
    }
}
