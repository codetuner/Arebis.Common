using System;
using System.Text;

namespace Arebis.Pdf.Common
{
    [Serializable]
    public class PdfASCIIHexDecodeFilter : PdfStreamFilter
    {
        private static readonly string HexChars = "0123456789ABCDEF";

        public PdfASCIIHexDecodeFilter()
            : base("/ASCIIHexDecode")
        { }


        public override byte[] Encode(byte[] bytes)
        {
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(HexChars[b >> 4]);
                builder.Append(HexChars[b & 15]);
            }
            builder.Append('>');
            return Encoding.ASCII.GetBytes(builder.ToString());
        }

        public override byte[] Decode(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
