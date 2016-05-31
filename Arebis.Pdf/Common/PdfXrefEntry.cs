using System;
using System.Globalization;

namespace Arebis.Pdf.Common
{
    [Serializable]
    public class PdfXrefEntry
    {
        private long offset;
        private int generation;
        private char type;

        public PdfXrefEntry(long offset, int generation, char type)
        {
            this.offset = offset;
            this.generation = generation;
            this.type = type;
        }

        public long Offset { get { return this.offset; } }

        public int Generation { get { return this.generation; } }

        public char Type { get { return this.type; } }

        public override string ToString()
        {
            return Offset.ToString("0000000000 ", CultureInfo.InvariantCulture) + Generation.ToString("00000 ", CultureInfo.InvariantCulture) + Type;
        }
    }
}
