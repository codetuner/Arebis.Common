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
    public static class FontSamples
    {
        private static PdfTextOptions CaptionText = new PdfTextOptions(PdfPredefinedFont.HelveticaBold, 12, PdfColor.Black);

        public static void Run(string outputfilename)
        {
            // Prepare document options:
            var options = new PdfDocumentOptions();
            options.Author = "Arebis";
            options.Title = "PDF Raster";
            options.Subject = "Demonstrate Arebis.Pdf generation library.";
            options.TextFilter = new Arebis.Pdf.Common.PdfDeflateStreamFilter();

            var sizes = new int[] { 48, 36, 32, 28, 24, 20, 18, 16, 14, 12, 10, 8, 6, 4 };

            // Make stream and writer objects:
            using (var stream = new FileStream(outputfilename, FileMode.Create, FileAccess.Write))
            using (var writer = new PdfDocumentWriter(stream, options))
            {
                using (var page = writer.NewPage(PdfPageFormat.A4Portrait))
                {
                    var cursor = page.Height - sizes[0] - 0;
                    cursor = WriteSampleLines(sizes, page, cursor, PdfPredefinedFont.Courier, "Courier");
                    cursor = WriteSampleLines(sizes, page, cursor, PdfPredefinedFont.CourierBold, "CourierBold");
                }

                using (var page = writer.NewPage(PdfPageFormat.A4Portrait))
                {
                    var cursor = page.Height - sizes[0] - 0;
                    cursor = WriteSampleLines(sizes, page, cursor, PdfPredefinedFont.CourierItalic, "CourierItalic");
                    cursor = WriteSampleLines(sizes, page, cursor, PdfPredefinedFont.CourierBoldItalic, "CourierBoldItalic");
                }

                using (var page = writer.NewPage(PdfPageFormat.A4Portrait))
                {
                    var cursor = page.Height - sizes[0] - 0;
                    cursor = WriteSampleLines(sizes, page, cursor, PdfPredefinedFont.Helvetica, "Helvetica");
                    cursor = WriteSampleLines(sizes, page, cursor, PdfPredefinedFont.HelveticaBold, "HelveticaBold");
                }

                using (var page = writer.NewPage(PdfPageFormat.A4Portrait))
                {
                    var cursor = page.Height - sizes[0] - 0;
                    cursor = WriteSampleLines(sizes, page, cursor, PdfPredefinedFont.HelveticaItalic, "HelveticaItalic");
                    cursor = WriteSampleLines(sizes, page, cursor, PdfPredefinedFont.HelveticaBoldItalic, "HelveticaBoldItalic");
                }

                using (var page = writer.NewPage(PdfPageFormat.A4Portrait))
                {
                    var cursor = page.Height - sizes[0] - 0;
                    cursor = WriteSampleLines(sizes, page, cursor, PdfPredefinedFont.TimesRoman, "TimesRoman");
                    cursor = WriteSampleLines(sizes, page, cursor, PdfPredefinedFont.TimesRomanBold, "TimesRomanBold");
                }

                using (var page = writer.NewPage(PdfPageFormat.A4Portrait))
                {
                    var cursor = page.Height - sizes[0] - 0;
                    cursor = WriteSampleLines(sizes, page, cursor, PdfPredefinedFont.TimesRomanItalic, "TimesRomanItalic");
                    cursor = WriteSampleLines(sizes, page, cursor, PdfPredefinedFont.TimesRomanBoldItalic, "TimesRomanBoldItalic");
                }
            }
        }

        private static double WriteSampleLines(int[] sizes, PdfPageWriter page, double cursor, PdfFont font, string fontName)
        {
            page.DrawText(40, cursor, String.Format("{0} in sizes {1}:", fontName, String.Join(", ", sizes.Select(s => s.ToString()))), CaptionText);
            cursor -= CaptionText.FontSize;
            cursor -= 32;
            foreach (var size in sizes)
            {
                page.DrawText(40, cursor, "01234-9 THE Quick BROWN FOX walks in tbe woods - "+ font.Name +  " " + size, new PdfTextOptions(font, size, PdfColor.Black));
                cursor -= size;
            }

            cursor -= 32;
            return cursor;
        }
    }
}
