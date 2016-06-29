using Arebis.Pdf.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Arebis.Pdf.Writing
{
    /// <summary>
    /// A PdfObject made with Page Markup Operations.
    /// </summary>
    [Serializable]
    public class PdfScriptObject : PdfObject
    {
        private const double ToRadiansFactor = 2.0 * Math.PI / 360.0;
        private const int MaxStrLen = 180;

        private StringBuilder streamContent;
        private bool isInTextBlock = false;

        public PdfScriptObject()
        {
            var textStream = new PdfTextStream();
            this.Stream = textStream;
            this.streamContent = textStream.Content;
            this.ReferencedFonts = new List<PdfFont>();
        }

        public IEnumerable<PdfFont> ReferencedFonts { get; private set; }

        /// <summary>
        /// Registers a font as referenced.
        /// </summary>
        protected void RegisterFont(PdfFont font)
        {
            if (!this.ReferencedFonts.Contains(font))
                ((List<PdfFont>)this.ReferencedFonts).Add(font);
        }

        /// <summary>
        /// Writes directly to the operators stream.
        /// </summary>
        public virtual void Write(string str)
        {
            this.streamContent.Append(str);
        }

        /// <summary>
        /// Writes directly to the operators stream.
        /// </summary>
        public virtual void WriteLine()
        {
            this.streamContent.Append('\n');
        }

        /// <summary>
        /// Writes directly to the operators stream.
        /// </summary>
        public void WriteLine(string str)
        {
            this.Write(str);
            this.WriteLine();
        }

        /// <summary>
        /// Writes comment to the operators stream.
        /// </summary>
        public virtual void WriteComment(string comment)
        {
            this.Write("% ");
            this.WriteLine(comment);
        }

        /// <summary>
        /// Saves the graphic state on the stack.
        /// </summary>
        public void BeginGraphicsState()
        {
            if (isInTextBlock) throw new InvalidOperationException("Must not be called between BeginText() and EndText().");
            this.WriteLine("q");
        }

        /// <summary>
        /// Restores the graphic state from the stack.
        /// </summary>
        public void EndGraphicsState()
        {
            if (isInTextBlock) throw new InvalidOperationException("Must not be called between BeginText() and EndText().");
            this.WriteLine("Q");
        }

        /// <summary>
        /// Concatenate given matrix to current transformation matrix.
        /// </summary>
        public void ConcatenateMatrix(PdfTransformationMatrix matrix)
        {
            this.ConcatenateMatrix(matrix.A, matrix.B, matrix.C, matrix.D, matrix.E, matrix.F);
        }

        /// <summary>
        /// Concatenate given matrix to current transformation matrix.
        /// </summary>
        public void ConcatenateMatrix(double a, double b, double c, double d, double e, double f)
        {
            this.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} cm", a, b, c, d, e, f));
        }

        /// <summary>
        /// Sets the line or stroke width.
        /// </summary>
        public void SetStrokeWidth(double value)
        {
            this.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0.###} w", value));
        }

        /// <summary>
        /// Sets the line cap style.
        /// </summary>
        public void SetLineCapStyle(PdfLineCapStyle value)
        {
            this.Write(((int)value).ToString());
            this.WriteLine(" J");
        }

        /// <summary>
        /// Sets the line join style.
        /// </summary>
        public void SetLineJoinStyle(PdfLineJoinStyle value)
        {
            this.Write(((int)value).ToString());
            this.WriteLine(" j");
        }

        /// <summary>
        /// Sets the line dash pattern.
        /// </summary>
        public void SetLineDashPattern(PdfLineDashPattern value)
        {
            this.Write(value.ToString());
            this.WriteLine(" d");
        }

        /// <summary>
        /// Sets the line dash pattern.
        /// </summary>
        public void SetLineDashPattern(int onUnits, int offUnits)
        {
            this.SetLineDashPattern(new PdfLineDashPattern(onUnits, offUnits));
        }

        /// <summary>
        /// Sets the drawing path beginning (and starts a new path).
        /// </summary>
        public void BeginPath(double x, double y)
        {
            if (isInTextBlock) throw new InvalidOperationException("Must not be called between BeginText() and EndText().");
            this.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0.###} {1:0.###} m", x, y));
        }

        /// <summary>
        /// Ends the drawing path and optionally closes, strokes and/or fills the path.
        /// </summary>
        /// <param name="closePath">Whether to close the path (draw a line from the last coordinates back to the path start).</param>
        /// <param name="strokePath">Whether to stroke the path (draw it's line).</param>
        /// <param name="fillPath">Whether to fill the path figure.</param>
        public void EndPath(bool closePath, bool strokePath, bool fillPath)
        {
            if (isInTextBlock) throw new InvalidOperationException("Must not be called between BeginText() and EndText().");
            if (closePath) this.Write("h ");
            if (strokePath && fillPath)
                this.WriteLine("B");
            else if (strokePath)
                this.WriteLine("S");
            else if (fillPath)
                this.WriteLine("f");
            else
                this.WriteLine("n");
        }

        /// <summary>
        /// Draws a line from the current path to the given coordinates.
        /// </summary>
        public void DrawLineTo(double x, double y)
        {
            if (isInTextBlock) throw new InvalidOperationException("Must not be called between BeginText() and EndText().");
            this.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0.###} {1:0.###} l", x, y));
        }

        /// <summary>
        /// Draws a line given 2 coordinates (convenience method).
        /// </summary>
        public void DrawLine(double x1, double y1, double x2, double y2)
        {
            this.BeginPath(x1, y1);
            this.DrawLineTo(x2, y2);
        }

        /// <summary>
        /// Draws a line from x, y with given angle and length (convenience method).
        /// </summary>
        public void DrawLineA(double x, double y, double angleInDegrees, double length)
        {
            double x2 = x + length * Math.Cos(angleInDegrees * ToRadiansFactor);
            double y2 = y + length * Math.Sin(angleInDegrees * ToRadiansFactor);
            this.DrawLine(x, y, x2, y2);
        }

        /// <summary>
        /// Draws a cubic bezier curve.
        /// </summary>
        public void DrawBezierCurve(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            if (isInTextBlock) throw new InvalidOperationException("Must not be called between BeginText() and EndText().");
            this.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0.###} {1:0.###} {2:0.###} {3:0.###} {4:0.###} {5:0.###} c", x1, y1, x2, y2, x3, y3));
        }

        /// <summary>
        /// Draws an oval or circle within the rectangle defined by the given coordinates (convenience method).
        /// </summary>
        public void DrawOval2(double x1, double y1, double x2, double y2)
        {
            var ray = (x2 - x1) / 2.0;
            var rf = 0.3333;
            var xm = x1 + ray;
            this.BeginPath(xm, y1);
            this.DrawBezierCurve(x1 - ray * rf, y1, x1 - ray * rf, y2, xm, y2);
            this.DrawBezierCurve(x2 + ray * rf, y2, x2 + ray * rf, y1, xm, y1);
        }

        /// <summary>
        /// Draws an oval or circle within the rectangle (convenience method).
        /// </summary>
        public void DrawOval(double x, double y, double width, double height)
        {
            this.DrawOval2(x, y, x + width, y + height);
        }

        /// <summary>
        /// Draws a circle with the given center coordinates and ray (convenience method).
        /// </summary>
        public void DrawCircle(double x, double y, double ray)
        {
            this.DrawOval2(x - ray, y - ray, x + ray, y + ray);
        }

        /// <summary>
        /// Draws a rectangle given width and height.
        /// </summary>
        public void DrawRectangle(double x, double y, double width, double height, double leftRotationDegrees = 0.0)
        {
            if (isInTextBlock) throw new InvalidOperationException("Must not be called between BeginText() and EndText().");
            if (leftRotationDegrees != 0.0)
            {
                this.Rotate(x, y, leftRotationDegrees);
                this.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0.###} {1:0.###} {2:0.###} {3:0.###} re", 0.0, 0.0, width, height));
            }
            else
            {
                this.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0.###} {1:0.###} {2:0.###} {3:0.###} re", x, y, width, height));
            }
        }

        /// <summary>
        /// Draws a rectangle given 2 opposite points (convenience method).
        /// </summary>
        public void DrawRectangle2(double x1, double y1, double x2, double y2, double leftRotationDegrees = 0.0)
        {
            this.DrawRectangle(x1, y1, x2 - x1, y2 - y1, leftRotationDegrees);
        }

        /// <summary>
        /// Draws a rounded rectangle given width, height and radius (convenience method).
        /// </summary>
        public void DrawRoundedRectangle(double x, double y, double width, double height, double radius)
        {
            this.DrawRoundedRectangle2(x, y, x + width, y + height, radius);
        }

        /// <summary>
        /// Draws a rounded rectangle given 2 opposite points and a radius.
        /// </summary>
        public void DrawRoundedRectangle2(double x1, double y1, double x2, double y2, double radius)
        {
            if (isInTextBlock) throw new InvalidOperationException("Must not be called between BeginText() and EndText().");

            var rf = 0.3;
            this.BeginPath(x1, y2 - radius);
            this.DrawBezierCurve(x1, y2 - radius * rf, x1 + radius * rf, y2, x1 + radius, y2);
            this.DrawLineTo(x2 - radius, y2);
            this.DrawBezierCurve(x2 - radius * rf, y2, x2, y2 - radius * rf, x2, y2 - radius);
            this.DrawLineTo(x2, y1 + radius);
            this.DrawBezierCurve(x2, y1 + radius * rf, x2 - radius * rf, y1, x2 - radius, y1);
            this.DrawLineTo(x1 + radius, y1);
            this.DrawBezierCurve(x1 + radius * rf, y1, x1, y1 + radius * rf, x1, y1 + radius);
        }

        /// <summary>
        /// Draws text.
        /// </summary>
        public void DrawText(string str)
        {
            if (!isInTextBlock) throw new InvalidOperationException("Must call BeginText() before this operation.");
            var addNewLine = false;
            str = str.Replace("\r\n", "\n");
            foreach (var part in str.Split('\n'))
            {
                if (addNewLine) this.WriteLine("T*");
                addNewLine = true;
                var substr = part;
                substr = substr.Replace(@"\", @"\\");
                substr = substr.Replace(@"(", @"\(");
                substr = substr.Replace(@")", @"\)");
                this.Write("(");
                while (substr.Length > MaxStrLen)
                {
                    var s = substr.Substring(0, MaxStrLen);
                    this.Write(s);
                    this.WriteLine(@"\");
                    substr = substr.Substring(MaxStrLen);
                }
                this.Write(substr);
                this.WriteLine(") Tj");
            }
        }

        /// <summary>
        /// Draws an XObject reference (i.e. to an image).
        /// </summary>
        public void DrawExternalObject(string objectName)
        {
            this.Write(objectName);
            this.WriteLine(" Do");
        }

        /// <summary>
        /// Draws an image (convenience method).
        /// </summary>
        public void DrawImageByName(double x, double y, double width, double height, string imageName, double leftRotationDegrees = 0.0)
        {
            this.BeginGraphicsState();
            this.ConcatenateMatrix(1, 0.0, 0.0, 1, x, y);
            if (leftRotationDegrees != 0.0) this.Rotate(0.0, 0.0, leftRotationDegrees);
            this.ConcatenateMatrix(width, 0.0, 0.0, height, 0, 0);
            this.DrawExternalObject(imageName);
            this.EndGraphicsState();
        }

        /// <summary>
        /// Draws an image (convenience method).
        /// </summary>
        public void DrawImageByName2(double x1, double y1, double x2, double y2, string imageName)
        {
            this.DrawImageByName(x1, y1, x2 - x1, y2 - y1, imageName);
        }

        /// <summary>
        /// Draws an image (convenience method).
        /// Assumes default naming of object references is used.
        /// </summary>
        public void DrawImageByRef(double x, double y, double width, double height, PdfObjectRef imageRef)
        {
            this.DrawImageByName(x, y, width, height, imageRef.ToDefaultName());
        }

        /// <summary>
        /// Draws an image (convenience method).
        /// Assumes default naming of object references is used.
        /// </summary>
        public void DrawImageByRef2(double x1, double y1, double x2, double y2, PdfObjectRef imageRef)
        {
            this.DrawImageByName2(x1, y1, x2, y2, imageRef.ToDefaultName());
        }

        /// <summary>
        /// Sets the stroke or pen color.
        /// </summary>
        public void SetStrokeColor(PdfColor color)
        {
            this.Write(color.ToString());
            this.WriteLine(" RG");
        }

        /// <summary>
        /// Sets the fill color.
        /// </summary>
        public void SetFillColor(PdfColor color)
        {
            this.Write(color.ToString());
            this.WriteLine(" rg");
        }

        /// <summary>
        /// Sets the text rendering mode.
        /// </summary>
        public void SetTextRenderingMode(PdfTextRenderingMode value)
        {
            if (!isInTextBlock) throw new InvalidOperationException("Must call BeginText() before this operation.");
            this.Write(((int)value).ToString());
            this.WriteLine(" Tr");
        }

        /// <summary>
        /// Sets the font type and size.
        /// </summary>
        public void SetFont(PdfFont font, double size)
        {
            if (!isInTextBlock) throw new InvalidOperationException("Must call BeginText() before this operation.");
            this.Write(font.Name);
            this.Write(" ");
            this.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0.##} Tf", size));
            this.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0.##} TL", size));
            this.RegisterFont(font);
        }

        /// <summary>
        /// Sets the text start position.
        /// </summary>
        public void SetTextStartPosition(double x, double y)
        {
            this.SetTextTransformationMatrix(1.0, 0.0, 0.0, 1.0, x, y);
        }

        /// <summary>
        /// Begins a block of text.
        /// </summary>
        public void BeginText()
        {
            if (isInTextBlock) throw new InvalidOperationException("Is already within a text block. Call EndText() first.");
            isInTextBlock = true;
            this.WriteLine("BT");
        }

        /// <summary>
        /// Begins a block of text with the given start position (convenience method).
        /// </summary>
        public void BeginText(double x, double y)
        {
            BeginText();
            SetTextStartPosition(x, y);
        }

        /// <summary>
        /// Begins a block of text with the given start position and rotation (convenience method).
        /// </summary>
        public void BeginText(double x, double y, double leftRotationDegrees)
        {
            BeginText();
            SetTextRotation(x, y, leftRotationDegrees);
        }

        /// <summary>
        /// Writes a transformation matrix for rotation (convenience method).
        /// </summary>
        public void SetTextRotation(double x, double y, double leftRotationDegrees)
        {
            SetTextTransformationMatrix(
                Math.Cos(leftRotationDegrees * ToRadiansFactor),
                Math.Sin(leftRotationDegrees * ToRadiansFactor),
                -Math.Sin(leftRotationDegrees * ToRadiansFactor),
                Math.Cos(leftRotationDegrees * ToRadiansFactor),
                x,
                y
            );
        }

        /// <summary>
        /// Modifies the transformation matrix for rotation of graphics (convenience method).
        /// </summary>
        public void Rotate(double x, double y, double leftRotationDegrees)
        {
            ModifyTransformationMatrix(
                Math.Cos(leftRotationDegrees * ToRadiansFactor),
                Math.Sin(leftRotationDegrees * ToRadiansFactor),
                -Math.Sin(leftRotationDegrees * ToRadiansFactor),
                Math.Cos(leftRotationDegrees * ToRadiansFactor),
                x,
                y
            );
        }

        /// <summary>
        /// Begins a block of text with the given start position and font (convenience method).
        /// </summary>
        public void BeginText(double x, double y, PdfFont font, double size)
        {
            BeginText(x, y);
            SetFont(font, size);
        }

        /// <summary>
        /// Begins a block of text with the given start position, rotation and font (convenience method).
        /// </summary>
        public void BeginText(double x, double y, double leftRotationDegrees, PdfFont font, double size)
        {
            BeginText(x, y, leftRotationDegrees);
            SetFont(font, size);
        }

        /// <summary>
        /// Begins a block of text with the given options (convenience method).
        /// </summary>
        public void BeginText(double x, double y, PdfTextOptions options)
        {
            BeginText();
            options.Apply(this, x, y);
        }

        /// <summary>
        /// Ends a block of text.
        /// </summary>
        public void EndText()
        {
            if (!isInTextBlock) throw new InvalidOperationException("Must call BeginText() before this operation.");
            this.WriteLine("ET");
            isInTextBlock = false;
        }

        /// <summary>
        /// Sets the text matrix to control position, rotation and skewing.
        /// </summary>
        public void SetTextTransformationMatrix(double a, double b, double c, double d, double e, double f)
        {
            if (!isInTextBlock) throw new InvalidOperationException("Must call BeginText() before this operation.");
            this.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} Tm", a, b, c, d, e, f));
        }

        /// <summary>
        /// Modifies the current transformation matrix to control position, rotation and skewing.
        /// </summary>
        public void ModifyTransformationMatrix(double a, double b, double c, double d, double e, double f)
        {
            //if (isInTextBlock) throw new InvalidOperationException("Must not be called between BeginText() and EndText().");
            this.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} cm", a, b, c, d, e, f));
        }

        /// <summary>
        /// Sets the text matrix to control position, rotation and skewing.
        /// </summary>
        public void SetTextTransformationMatrix(PdfTransformationMatrix matrix)
        {
            this.SetTextTransformationMatrix(matrix.A, matrix.B, matrix.C, matrix.D, matrix.E, matrix.F);
        }

        public void SetTextOptions(PdfTextOptions textOptions, double x, double y)
        {
            textOptions.Apply(this, x, y);
        }

        public void SetGraphicsOptions(PdfGraphicsOptions graphicsOptions)
        {
            graphicsOptions.Apply(this);
        }
    }
}
