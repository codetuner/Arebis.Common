using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Pdf.Writing
{
    public interface ITextTransformer
    {
        /// <summary>
        /// Transforms (or not) text to be written in a Pdf document.
        /// </summary>
        /// <param name="text">The text to transform.</param>
        /// <returns>The transformed text.</returns>
        string Transform(string text);
    }
}
