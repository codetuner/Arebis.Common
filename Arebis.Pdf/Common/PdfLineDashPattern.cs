using System;
using System.Globalization;

namespace Arebis.Pdf.Common
{
    [Serializable]
    public class PdfLineDashPattern
    {
        private string str;

        public PdfLineDashPattern(int units)
            : this(units, units)
        { }

        public PdfLineDashPattern(int onUnits, int offUnits, int startPhase = 0)
            : this(String.Format(CultureInfo.InvariantCulture, "[{0} {1}] {2}", onUnits, offUnits, startPhase))
        { }

        public PdfLineDashPattern(string str)
        {
            this.str = str;
        }

        public override string ToString()
        {
            return this.str;
        }

        public static readonly PdfLineDashPattern Solid = new PdfLineDashPattern("[] 0");
        public static readonly PdfLineDashPattern Small = new PdfLineDashPattern("[2] 0");
        public static readonly PdfLineDashPattern Medium = new PdfLineDashPattern("[4] 0");
        public static readonly PdfLineDashPattern Large = new PdfLineDashPattern("[8] 0");
        public static readonly PdfLineDashPattern XLarge = new PdfLineDashPattern("[16] 0");
    }
}
