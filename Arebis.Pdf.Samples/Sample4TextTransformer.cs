using Arebis.Pdf.Writing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Pdf.Samples
{
    /// <summary>
    /// A sample ITextTransformer implementation transforming text to uppercase.
    /// </summary>
    public class Sample4TextTransformer : ITextTransformer
    {
        public string Transform(string text)
        {
            return text.ToUpper();
        }
    }
}
