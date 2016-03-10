using Arebis.Pdf.Common;
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
            var obj = new PdfScriptObject();
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
            var obj = new PdfScriptObject();
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
        public static PdfObjectRef DrawRectangle(this PdfPageWriter page, double x, double y, double width, double height, PdfGraphicsOptions options)
        {
            var obj = new PdfScriptObject();
            obj.BeginGraphicsState();
            options.Apply(obj);
            obj.DrawRectangle(x, y, width, height);
            obj.EndPath(true, true, true);
            obj.EndGraphicsState();
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws a rectangle given 2 coordinates.
        /// </summary>
        public static PdfObjectRef DrawRectangle2(this PdfPageWriter page, double x1, double y1, double x2, double y2, PdfGraphicsOptions options)
        {
            var obj = new PdfScriptObject();
            obj.BeginGraphicsState();
            options.Apply(obj);
            obj.DrawRectangle2(x1, y1, x2, y2);
            obj.EndPath(true, true, true);
            obj.EndGraphicsState();
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws an oval given left-upper coordinates and width and height.
        /// </summary>
        public static PdfObjectRef DrawOval(this PdfPageWriter page, double x, double y, double width, double height, PdfGraphicsOptions options)
        {
            var obj = new PdfScriptObject();
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
            var obj = new PdfScriptObject();
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
        public static PdfObjectRef DrawImageRef(this PdfPageWriter page, double x, double y, PdfObjectRef imageRef, double width, double height, PdfImagePlacement imagePlacement)
        {
            if (imagePlacement != PdfImagePlacement.Stretch)
            {
                var boxar = height / width;
                var imgar = page.DocumentWriter.GetImageHeight(imageRef, 1.0);
                if (imgar < boxar)
                {
                    var imgh = height / boxar * imgar;
                    switch (imagePlacement)
                    {
                        case PdfImagePlacement.LeftOrTop:
                            y = y + (height - imgh);
                            break;
                        case PdfImagePlacement.Center:
                            y = y + (height - imgh) / 2.0;
                            break;
                    }
                    height = imgh;
                }
                else
                {
                    var imgw = width * boxar / imgar;
                    switch (imagePlacement)
                    {
                        case PdfImagePlacement.RightOrBottom:
                            x = x + (width - imgw);
                            break;
                        case PdfImagePlacement.Center:
                            x = x + (width - imgw) / 2.0;
                            break;
                    }
                    width = imgw;
                }
            }
            var obj = new PdfScriptObject();
            obj.DrawImageByName(x, y, width, height, page.DocumentWriter.GetNameOfXObject(imageRef));
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws an image previously added to the DocumentWriter,
        /// stretched to the given width and height.
        /// </summary>
        public static PdfObjectRef DrawImageRef(this PdfPageWriter page, double x, double y, PdfObjectRef imageRef, double width)
        {
            return page.DrawImageRef(x, y, imageRef, width, page.DocumentWriter.GetImageHeight(imageRef, width), PdfImagePlacement.Stretch);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws an image,
        /// scaled to the given width and height.
        /// </summary>
        public static PdfObjectRef DrawImage(this PdfPageWriter page, double x, double y, Image image, double width, double height, PdfImagePlacement imagePlacement)
        {
            var imageRef = page.DocumentWriter.AddImage(image);
            return page.DrawImageRef(x, y, imageRef, width, height, imagePlacement);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws an image,
        /// stretched to the given width and height.
        /// </summary>
        public static PdfObjectRef DrawImage(this PdfPageWriter page, double x, double y, Image image, double width)
        {
            var imageRef = page.DocumentWriter.AddImage(image);
            return page.DrawImageRef(x, y, imageRef, width);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws an XObject.
        /// </summary>
        public static PdfObjectRef DrawXObjectRef(this PdfPageWriter page, double x, double y, PdfObjectRef xObjRef, double width)
        {
            var obj = new PdfScriptObject();
            obj.DrawExternalObject(page.DocumentWriter.GetNameOfXObject(xObjRef));
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws text.
        /// </summary>
        public static PdfObjectRef DrawText(this PdfPageWriter page, double x, double y, string str, PdfTextOptions options)
        {
            var obj = new PdfScriptObject();
            obj.BeginGraphicsState();
            obj.BeginText(x, y, options);
            obj.DrawText(str);
            obj.EndText();
            obj.EndGraphicsState();
            return page.WriteObject(obj);
        }

        /// <summary>
        /// Adds a PdfScriptObject to the page that draws text that autowraps at the given blockWidth.
        /// </summary>
        public static PdfObjectRef DrawTextblock(this PdfPageWriter page, double x, double y, string str, double blockWidth, PdfTextOptions options)
        {
            return page.DrawText(x, y, options.Font.SplitText(str, options.FontSize, blockWidth), options);
        }
    }
}
