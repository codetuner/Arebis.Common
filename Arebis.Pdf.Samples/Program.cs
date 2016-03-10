using Arebis.Pdf.Writing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Pdf.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            HelloWorldSample.Run(@"HelloWorld.pdf");
            Sample1.Run(@"Sample1.pdf");
            RasterPage.Run(@"RasterA4Portrait.pdf", PdfPageFormat.A4Portrait);
            RasterPage.Run(@"RasterA4Landscape.pdf", PdfPageFormat.A4Landscape);
        }
    }
}
