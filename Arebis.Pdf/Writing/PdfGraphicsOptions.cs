using Arebis.Pdf.Common;
using System;

namespace Arebis.Pdf.Writing
{
    [Serializable]
    public class PdfGraphicsOptions
    {
        public PdfGraphicsOptions(double strokeWidth = 0.0, PdfColor strokeColor = null, PdfColor fillColor = null, PdfLineDashPattern lineStyle = null, PdfLineCapStyle? lineCapStyle = null, PdfLineJoinStyle? lineJoinStyle = null)
        {
            this.StrokeColor = strokeColor ?? PdfColor.Black;
            this.FillColor = fillColor ?? PdfColor.White;
            this.LineDashPattern = lineStyle ?? PdfLineDashPattern.Solid;
            this.StrokeWidth = strokeWidth;
            this.LineCapstyle = lineCapStyle;
            this.LineJoinStyle = lineJoinStyle;
        }

        public PdfGraphicsOptions(PdfGraphicsOptions template)
        {
            this.StrokeColor = template.StrokeColor;
            this.FillColor = template.FillColor;
            this.LineDashPattern = template.LineDashPattern;
            this.StrokeWidth = template.StrokeWidth;
            this.LineCapstyle = template.LineCapstyle;
            this.LineJoinStyle = template.LineJoinStyle;
        }

        public double StrokeWidth { get; set; }

        public PdfColor StrokeColor { get; set; }

        public PdfColor FillColor { get; set; }

        public PdfLineDashPattern LineDashPattern { get; set; }

        public PdfLineCapStyle? LineCapstyle { get; set; }

        public PdfLineJoinStyle? LineJoinStyle { get; set; }

        /// <summary>
        /// Applies these options to the given script object.
        /// </summary>
        protected internal virtual void Apply(PdfScriptObject onObject)
        {
            onObject.SetStrokeColor(this.StrokeColor);
            onObject.SetFillColor(this.FillColor);
            onObject.SetStrokeWidth(this.StrokeWidth);
            onObject.SetLineDashPattern(this.LineDashPattern);
            if (this.LineCapstyle.HasValue) onObject.SetLineCapStyle(this.LineCapstyle.Value);
            if (this.LineJoinStyle.HasValue) onObject.SetLineJoinStyle(this.LineJoinStyle.Value);
        }
    }
}
