using System;

namespace Arebis.Pdf.Common
{
    [Serializable]
    public enum PdfTextRenderingMode
    {
        Fill = 0,
        Stroke = 1,
        FillAndStroke = 2,
        Invisible = 3,
        FillAndClipPath = 4,
        StrokeAndClipPath = 5,
        FillStrokeAndClipPath = 6,
        ClipPath = 7,
    }
}
