using Arebis.Pdf.Common;
using Arebis.Pdf.Writing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Pdf.Samples
{
    public static class RasterPage
    {
        public static void Run(string outputfilename, PdfPageFormat pageFormat)
        {
            // Prepare document options:
            var options = new PdfDocumentOptions();
            options.Author = "Arebis";
            options.Title = "PDF Raster";
            options.Subject = "Demonstrate Arebis.Pdf generation library.";
            options.TextFilter = new Arebis.Pdf.Common.PdfDeflateStreamFilter();
            //options.TextFilter = new Arebis.Pdf.Common.PdfASCIIHexDecodeFilter();

			// Content options:
			var graphicsOptions = new PdfGraphicsOptions(0.1, PdfColor.Black, null, PdfLineDashPattern.Small);
			var textOptions = new PdfTextOptions(PdfPredefinedFont.Helvetica, 8, PdfColor.Black);

            // Make stream and writer objects:
            using (var stream = new FileStream(outputfilename, FileMode.Create, FileAccess.Write))
            using (var writer = new PdfDocumentWriter(stream, options))
            {
                using (var page = writer.NewPage(pageFormat))
                {
                    for (int x = 20; x < page.Width; x += 20)
                    {
                        page.DrawLine(x, 0, x, page.Height, graphicsOptions);
                        page.DrawText(x + 1, 101, x.ToString(), textOptions);
                    }

                    for (int y = 20; y < page.Height; y += 20)
                    {
                        page.DrawLine(0, y, page.Width, y, graphicsOptions);
                        page.DrawText(101, y - 8, y.ToString(), textOptions);
                    }
                }
            }
        }
    }
}
