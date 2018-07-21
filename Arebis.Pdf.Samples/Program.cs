using Arebis.Pdf.Writing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Process.Start(@"HelloWorld.pdf");

            Sample1.Run(@"Sample1.pdf");
            Process.Start(@"Sample1.pdf");

            Sample2.Run(@"Sample2.xaml", @"Sample2.pdf");
            Process.Start(@"Sample2.pdf");

            Sample3.Run(@"Sample3.pdf");
            Process.Start(@"Sample3.pdf");

            Sample4.Run(@"Sample4.pdf");
            Process.Start(@"Sample4.pdf");

            FontSamples.Run(@"FontSamples.pdf");
            Process.Start(@"FontSamples.pdf");

            RasterPage.Run(@"RasterA4Portrait.pdf", PdfPageFormat.A4Portrait);
            Process.Start(@"RasterA4Portrait.pdf");

            RasterPage.Run(@"RasterA4Landscape.pdf", PdfPageFormat.A4Landscape);
            Process.Start(@"RasterA4Landscape.pdf");
        }
    }
}
