using Arebis.Pdf.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;

namespace Arebis.Pdf.Samples
{
    public static class Sample2
    {
        public static void Run(string inputfilename, string outputfilename)
        { 
            // Load the document from file:
            var document = (Document)XamlServices.Load(inputfilename);

            // Write the document to PDF:
            PdfModelWriter.WriteDocument(document, outputfilename);
        }
    }
}
