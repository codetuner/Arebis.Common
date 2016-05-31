using System;
using System.IO;
using System.Text;

namespace Arebis.Pdf.Common
{
    public class PdfDeflateStreamFilter : PdfStreamFilter
    {
        public PdfDeflateStreamFilter()
            : base("/FlateDecode")
        { }

        public override byte[] Encode(byte[] bytes)
        {
            using (var inputstr = new MemoryStream(bytes))
            using (var outputstr = new MemoryStream())
            {
                // http://stackoverflow.com/questions/9050260/what-does-a-zlib-header-look-like
                // http://stackoverflow.com/questions/18450297/is-it-possible-to-use-the-net-deflatestream-for-pdf-creation
                // https://web.archive.org/web/20130905215303/http://connect.microsoft.com/VisualStudio/feedback/details/97064/deflatestream-throws-exception-when-inflating-pdf-streams

//#if IONIC
                using (var dstream = new Ionic.Zlib.ZlibStream(outputstr, Ionic.Zlib.CompressionMode.Compress))
                {
                    dstream.FlushMode = Ionic.Zlib.FlushType.Finish;
//#elif NET40
//                using (var dstream = new System.IO.Compression.DeflateStream(outputstr, System.IO.Compression.CompressionMode.Compress))
//                {
//                    // Zlib magic header (Default Compression):
//                    outputstr.WriteByte(0x78);
//                    outputstr.WriteByte(0x9C);
//#else //NET45+
//                using (var dstream = new System.IO.Compression.DeflateStream(outputstr, System.IO.Compression.CompressionLevel.Optimal))
//                {
//                    // Zlib magic header (Best Compression):
//                    outputstr.WriteByte(0x78);
//                    outputstr.WriteByte(0xDA);
//#endif
                    inputstr.CopyTo(dstream);
                    inputstr.Flush();
                }
                return outputstr.ToArray();
            }
        }

        public override byte[] Decode(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
