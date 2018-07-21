using Arebis.Pdf.Common;
using System;
using System.Drawing;

namespace Arebis.Pdf.Writing
{
    public static class PdfPageWriterExtensions
    {
        /// <summary>
        /// Adds a PdfScriptObject to the page that draws a circle.
        /// </summary>
        public static PdfObjectRef DrawCircle(this PdfPageWriter page, double x, double y, double ray, PdfGraphicsOptions options)
        {
            var obj = page.DocumentWriter.CreatePdfScriptObject();
            obj.BeginGraphicsState();
            options.Apply(obj);
            obj.DrawCircle(x, y, ray);
            obj.EndPath(true, true, true);
            obj.EndGraphicsState();
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws a line.
        /// </summary>
        public static PdfObjectRef DrawLine(this PdfPageWriter page, double x1, double y1, double x2, double y2, PdfGraphicsOptions options)
        {
            var obj = page.DocumentWriter.CreatePdfScriptObject();
            obj.BeginGraphicsState();
            options.Apply(obj);
            obj.BeginPath(x1, y1);
            obj.DrawLineTo(x2, y2);
            obj.EndPath(false, true, true);
            obj.EndGraphicsState();
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws a rectangle given left-upper coordinates and width and height.
        /// </summary>
        public static PdfObjectRef DrawRectangle(this PdfPageWriter page, double x, double y, double width, double height, PdfGraphicsOptions options, double leftRotationDegrees = 0.0)
        {
            var obj = page.DocumentWriter.CreatePdfScriptObject();
            obj.BeginGraphicsState();
            options.Apply(obj);
            obj.DrawRectangle(x, y, width, height, leftRotationDegrees);
            obj.EndPath(true, true, true);
            obj.EndGraphicsState();
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws a rectangle given 2 coordinates.
        /// </summary>
        public static PdfObjectRef DrawRectangle2(this PdfPageWriter page, double x1, double y1, double x2, double y2, PdfGraphicsOptions options, double leftRotationDegrees = 0.0)
        {
            var obj = page.DocumentWriter.CreatePdfScriptObject();
            obj.BeginGraphicsState();
            options.Apply(obj);
            obj.DrawRectangle2(x1, y1, x2, y2, leftRotationDegrees);
            obj.EndPath(true, true, true);
            obj.EndGraphicsState();
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws a rounded rectangle given left-upper coordinates and width and height.
        /// </summary>
        public static PdfObjectRef DrawRoundedRectangle(this PdfPageWriter page, double x, double y, double width, double height, double radius, PdfGraphicsOptions options)
        {
            var obj = page.DocumentWriter.CreatePdfScriptObject();
            obj.BeginGraphicsState();
            options.Apply(obj);
            obj.DrawRoundedRectangle(x, y, width, height, radius);
            obj.EndPath(true, true, true);
            obj.EndGraphicsState();
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws a rounded rectangle given 2 coordinates.
        /// </summary>
        public static PdfObjectRef DrawRoundedRectangle2(this PdfPageWriter page, double x1, double y1, double x2, double y2, double radius, PdfGraphicsOptions options)
        {
            var obj = page.DocumentWriter.CreatePdfScriptObject();
            obj.BeginGraphicsState();
            options.Apply(obj);
            obj.DrawRoundedRectangle2(x1, y1, x2, y2, radius);
            obj.EndPath(true, true, true);
            obj.EndGraphicsState();
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws an oval given left-upper coordinates and width and height.
        /// </summary>
        public static PdfObjectRef DrawOval(this PdfPageWriter page, double x, double y, double width, double height, PdfGraphicsOptions options)
        {
            var obj = page.DocumentWriter.CreatePdfScriptObject();
            obj.BeginGraphicsState();
            options.Apply(obj);
            obj.DrawOval(x, y, width, height);
            obj.EndPath(true, true, true);
            obj.EndGraphicsState();
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws a oval given 2 coordinates.
        /// </summary>
        public static PdfObjectRef DrawOval2(this PdfPageWriter page, double x1, double y1, double x2, double y2, PdfGraphicsOptions options)
        {
            var obj = page.DocumentWriter.CreatePdfScriptObject();
            obj.BeginGraphicsState();
            options.Apply(obj);
            obj.DrawOval2(x1, y1, x2, y2);
            obj.EndPath(true, true, true);
            obj.EndGraphicsState();
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws an image previously added to the DocumentWriter,
        /// scaled to the given width and height.
        /// </summary>
        public static PdfObjectRef DrawImageRef(this PdfPageWriter page, double x, double y, PdfObjectRef imageRef, double width, double height, PdfImagePlacement imagePlacement, PdfImageRotation rotation = PdfImageRotation.None)
        {
            var width2 = width;
            var height2 = height;
            var xdelta = 0.0;
            var ydelta = 0.0;

            // Fix for rotations:
            if (rotation == PdfImageRotation.Left || rotation == PdfImageRotation.Right) Swap(ref width2, ref height2);
            imagePlacement = imagePlacement.Rotated(rotation);

            if (imagePlacement != PdfImagePlacement.Stretch)
            {
                // Calculate box aspect ratio and image aspect ratio:
                var boxar = height2 / width2;
                var imgar = page.DocumentWriter.GetImageHeight(imageRef, 1.0);

                if (imgar < boxar)
                {
                    var imgh = height2 / boxar * imgar;
                    switch (imagePlacement)
                    {
                        case PdfImagePlacement.LeftOrTop:
                        case PdfImagePlacement.RightOrTop:
                            ydelta = (height2 - imgh);
                            break;
                        case PdfImagePlacement.Center:
                            ydelta = (height2 - imgh) / 2.0;
                            break;
                    }
                    height2 = imgh;
                }
                else
                {
                    var imgw = width2 * boxar / imgar;
                    switch (imagePlacement)
                    {
                        case PdfImagePlacement.RightOrBottom:
                            xdelta = (width2 - imgw);
                            break;
                        case PdfImagePlacement.RightOrTop:
                            xdelta = (width2 - imgw);
                            break;
                        case PdfImagePlacement.Center:
                            xdelta = (width2 - imgw) / 2.0;
                            break;
                    }
                    width2 = imgw;
                }
            }

            // Correct for rotations:
            switch (rotation)
            {
                case PdfImageRotation.None: break;
                case PdfImageRotation.Left:
                    Swap(ref xdelta, ref ydelta);
                    xdelta = width - xdelta;
                    break;
                case PdfImageRotation.Right:
                    Swap(ref xdelta, ref ydelta);
                    ydelta = height - ydelta;
                    break;
                case PdfImageRotation.UpsideDown:
                    xdelta = width - xdelta;
                    ydelta = height - ydelta;
                    break;
            }

            var obj = page.DocumentWriter.CreatePdfScriptObject();
            obj.DrawImageByName(x + xdelta, y + ydelta, width2, height2, page.DocumentWriter.GetNameOfXObject(imageRef), ((int)rotation) * 90.0);
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws an image previously added to the DocumentWriter,
        /// stretched to the given width and height.
        /// </summary>
        public static PdfObjectRef DrawImageRef(this PdfPageWriter page, double x, double y, PdfObjectRef imageRef, double width, double leftRotationDegrees = 0.0)
        {
            var obj = page.DocumentWriter.CreatePdfScriptObject();
            obj.DrawImageByName(x, y, width, page.DocumentWriter.GetImageHeight(imageRef, width), page.DocumentWriter.GetNameOfXObject(imageRef), leftRotationDegrees);
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws an image,
        /// scaled to the given width and height.
        /// </summary>
        public static PdfObjectRef DrawImage(this PdfPageWriter page, double x, double y, Image image, double width, double height, PdfImagePlacement imagePlacement, PdfImageRotation rotation = PdfImageRotation.None)
        {
            var imageRef = page.DocumentWriter.AddImage(image);
            return page.DrawImageRef(x, y, imageRef, width, height, imagePlacement, rotation);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws an image,
        /// stretched to the given width and height.
        /// </summary>
        public static PdfObjectRef DrawImage(this PdfPageWriter page, double x, double y, Image image, double width, double leftRotationDegrees = 0.0)
        {
            var imageRef = page.DocumentWriter.AddImage(image);
            return page.DrawImageRef(x, y, imageRef, width, leftRotationDegrees);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws an XObject.
        /// </summary>
        public static PdfObjectRef DrawXObjectRef(this PdfPageWriter page, double x, double y, PdfObjectRef xObjRef, double width)
        {
            var obj = page.DocumentWriter.CreatePdfScriptObject();
            obj.DrawExternalObject(page.DocumentWriter.GetNameOfXObject(xObjRef));
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws text.
        /// </summary>
        public static PdfObjectRef DrawText(this PdfPageWriter page, double x, double y, string text, PdfTextOptions options, double leftRotationDegrees = 0.0)
        {
            var obj = page.DocumentWriter.CreatePdfScriptObject();
            obj.BeginGraphicsState();
            obj.BeginText(x, y, options);
            if (leftRotationDegrees != 0.0) obj.SetTextRotation(x, y, leftRotationDegrees);
            obj.DrawText(text);
            obj.EndText();
            obj.EndGraphicsState();
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws text that autowraps at the given blockWidth.
        /// </summary>
        /// <param name="page">Page to draw to.</param>
        /// <param name="x">X coordinate left-bottom of the first line of text.</param>
        /// <param name="y">Y coordinate left-bottom of the first line of text.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="blockWidth">Width of the text block.</param>
        /// <param name="options">Text options (font, style, etc.)</param>
        /// <param name="leftRotationDegrees">Left rotation expressed in degrees.</param>
        /// <param name="align">Alignment: -1=left (default), 0=center, +1=right.</param>
        /// <remarks>
        /// Currently, when alignment is non-left, the rotation arguments is ignored as both are not supported together.
        /// </remarks>
        public static PdfObjectRef DrawTextblock(this PdfPageWriter page, double x, double y, string text, double blockWidth, PdfTextOptions options, double leftRotationDegrees = 0.0, int align = -1)
        {
            if (align < 0)
            {
                return page.DrawText(x, y, options.Font.SplitText(text, options.FontSize, blockWidth), options, leftRotationDegrees);
            }
            else
            {
                var lines = options.Font.SplitText(text.Replace('\r', ' '), options.FontSize, blockWidth).Split('\n');
                var obj = page.DocumentWriter.CreatePdfScriptObject();
                obj.BeginGraphicsState();
                obj.BeginText(x, y, options);
                // Centered or right-alignment is not compatible with rotation:
                //if (leftRotationDegrees != 0.0) obj.SetTextRotation(x, y, leftRotationDegrees);
                var liney = y;
                foreach (var line in lines)
                {
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        var tline = line.Trim();
                        var textwidth = options.Font.GetStringWidth(tline, options.FontSize);
                        var linex = (align > 0) ? (x + blockWidth - textwidth) : (x + (blockWidth - textwidth) / 2);
                        obj.SetTextStartPosition(linex, liney);
                        obj.DrawText(tline);
                    }
                    liney -= options.FontSize;
                }
                obj.EndText();
                obj.EndGraphicsState();
                return page.WriteObject(obj);
            }
        }

        /// <summary>
        /// Swaps the values of the two given variables.
        /// </summary>
        private static void Swap<T>(ref T a, ref T b)
        {
            T x = a;
            a = b;
            b = x;
        }
    }
}
