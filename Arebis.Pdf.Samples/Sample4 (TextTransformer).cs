﻿using Arebis.Pdf.Common;
using Arebis.Pdf.Writing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Pdf.Samples
{
    public static class Sample4
    {
        public static void Run(string filename)
        {
            var documentOptions = new PdfDocumentOptions();
            documentOptions.TextTransformer = new Sample4TextTransformer();

            // Create a TextOptions object:
            var to = new PdfTextOptions(PdfPredefinedFont.Helvetica, 12.0, PdfColor.Blue);

            // Write the document:
            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            using (var writer = new PdfDocumentWriter(stream, documentOptions))
            {
                // Write a page:
                using (var page = writer.NewPage(PdfPageFormat.A4Portrait))
                {
                    // Draw text on the page (with given TextOptions):
                    page.DrawText(40, 800, "Hello World !", to);
                }
            }
        }
    }
}
