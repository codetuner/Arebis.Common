using Arebis.Pdf.Common;
using System;

namespace Arebis.Pdf.Writing
{
    [Serializable]
    public class PdfTextOptions
    {
        public PdfTextOptions()
            : this(PdfPredefinedFont.Helvetica, 12.0)
        { }

        public PdfTextOptions(PdfFont font, double fontSize, PdfColor inkColor = null, PdfTextRenderingMode renderingMode = PdfTextRenderingMode.Fill, PdfColor outlineColor = null, double? outlineWidth = null, PdfLineDashPattern lineDashPattern = null, PdfLineCapStyle? lineCapStyle = null)
        {
            this.InkColor = inkColor ?? PdfColor.Black;
            this.Font = font;
            this.FontSize = fontSize;
            this.LeftRotationDegrees = 0;
            this.RenderingMode = renderingMode;
            this.OutlineColor = outlineColor;
            this.LineDashPattern = lineDashPattern;
            this.LineCapStyle = LineCapStyle;
            this.OutlineWidth = outlineWidth;
        }

        [Obsolete("Since Arebis.Pdf 1.4, the LeftRotationDegrees property is being outphased from the PdfTextOptions and replaced by an extra argument on DrawText methods for consistency with graphics methods.")]
        public PdfTextOptions(PdfFont font, double fontSize, PdfColor inkColor, int leftRotationDegrees, PdfTextRenderingMode renderingMode = PdfTextRenderingMode.Fill, PdfColor outlineColor = null, double? outlineWidth = null, PdfLineDashPattern lineDashPattern = null, PdfLineCapStyle? lineCapStyle = null)
        {
            this.InkColor = inkColor ?? PdfColor.Black;
            this.Font = font;
            this.FontSize = fontSize;
            this.LeftRotationDegrees = leftRotationDegrees;
            this.RenderingMode = renderingMode;
            this.OutlineColor = outlineColor;
            this.LineDashPattern = lineDashPattern;
            this.LineCapStyle = LineCapStyle;
            this.OutlineWidth = outlineWidth;
        }

        public PdfTextOptions(PdfTextOptions template)
        {
            this.InkColor = template.InkColor;
            this.Font = template.Font;
            this.FontSize = template.FontSize;
            this.LeftRotationDegrees = template.LeftRotationDegrees;
            this.RenderingMode = template.RenderingMode;
            this.OutlineColor = template.OutlineColor;
            this.OutlineWidth = template.OutlineWidth;
            this.LineDashPattern = template.LineDashPattern;
            this.LineCapStyle = template.LineCapStyle;
        }

        /// <summary>
        /// Ink in which the font is written.
        /// </summary>
        public PdfColor InkColor { get; set; }

        /// <summary>
        /// Outline color of the font (requires stroking RenderingMode).
        /// </summary>
        public PdfColor OutlineColor { get; set; }

        /// <summary>
        /// Font type.
        /// </summary>
        public PdfFont Font { get; set; }

        /// <summary>
        /// Size of the font.
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// Number of degrees to rotate the text to the left.
        /// </summary>
        [Obsolete("Since Arebis.Pdf 1.4, the LeftRotationDegrees property is being outphased from the PdfTextOptions and replaced by an extra argument on DrawText methods for consistency with graphics methods.")]
        public int LeftRotationDegrees { get; set; }

        /// <summary>
        /// Rendering mode of the text.
        /// </summary>
        public PdfTextRenderingMode RenderingMode { get; set; }

        public PdfLineDashPattern LineDashPattern { get; set; }

        public PdfLineCapStyle? LineCapStyle { get; set; }

        public double? OutlineWidth { get; set; }

        /// <summary>
        /// Applies these options to the given script object, and sets initial coordinates.
        /// </summary>
        protected internal virtual void Apply(PdfScriptObject onObject, double x, double y)
        {
            onObject.SetFillColor(this.InkColor);
            onObject.SetFont(this.Font, this.FontSize);
            if (this.LeftRotationDegrees != 0)
                onObject.SetTextRotation(x, y, this.LeftRotationDegrees);
            else
                onObject.SetTextStartPosition(x, y);
            onObject.SetTextRenderingMode(this.RenderingMode);
            if (this.OutlineColor != null)
                onObject.SetStrokeColor(this.OutlineColor);
            if (this.LineDashPattern != null)
                onObject.SetLineDashPattern(this.LineDashPattern);
            if (this.LineCapStyle.HasValue)
                onObject.SetLineCapStyle(this.LineCapStyle.Value);
            if (this.OutlineWidth.HasValue)
                onObject.SetStrokeWidth(this.OutlineWidth.Value);
        }

        public double GetStringWidth(string str)
        {
            return this.Font.GetStringWidth(str, this.FontSize);
        }

        public String SplitText(string text, double width)
        {
            return this.Font.SplitText(text, this.FontSize, width);
        }

        public string TrimLength(string str, double width)
        {
            return this.Font.TrimLength(str, this.FontSize, width);
        }
    }
}
