using Arebis.Pdf.Common;
using Arebis.Pdf.Model;
using Arebis.Pdf.Writing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Net;

namespace Arebis.Pdf.Model
{
    public class PdfModelWriter
    {
        private static Regex valuesRegex = new Regex("([0-9]*\\.?[0-9]+)(.*)", RegexOptions.Compiled);

        private Dictionary<string, object> referables = new Dictionary<string, object>();

        /// <summary>
        /// Resolves ContentKey values of Text and TextBlock elements.
        /// </summary>
        public IDictionary<string, string> ContentSource { get; set; }

        /// <summary>
        /// Callback action called when starting a document.
        /// </summary>
        public Action<PdfDocumentWriter, Document> OnDocumentBegin { get; set; }

        /// <summary>
        /// Callback action called when finalizing a document.
        /// </summary>
        public Action<PdfDocumentWriter, Document> OnDocumentEnd { get; set; }

        /// <summary>
        /// Callback action called when starting a new page.
        /// </summary>
        public Action<PdfPageWriter, Page> OnPageBegin { get; set; }

        /// <summary>
        /// Callback action called when finalizing a page.
        /// </summary>
        public Action<PdfPageWriter, Page> OnPageEnd { get; set; }

        /// <summary>
        /// Convenience method to write a document at once to a file.
        /// </summary>
        /// <param name="document">The document to write.</param>
        /// <param name="toFilename">The filename to write to.</param>
        /// <param name="contentSource">An optional content source to resolve ContentKey values of Text and TextBlock elements.</param>
        public static void WriteDocument(Document document, string toFilename, IDictionary<string, string> contentSource = null)
        {
            using (var stream = new FileStream(toFilename, FileMode.Create, FileAccess.Write))
            {
                var writer = new PdfModelWriter();
                writer.ContentSource = contentSource;
                writer.Write(document, stream);
            }
        }

        /// <summary>
        /// Writes a PDF document based on the given document object to the given stream.
        /// </summary>
        public void Write(Document document, System.IO.FileStream stream)
        {
            var options = new PdfDocumentOptions();
            options.Title = document.Title;
            options.Subject = document.Subject;
            options.Keywords = document.Keywords;
            options.Author = document.Author;
            options.TextFilter = new Arebis.Pdf.Common.PdfDeflateStreamFilter();

            var context = new Context();
            context.GraphicsOptionsRef = document.GraphicsOptionsRef;
            context.TextOptionsRef = document.TextOptionsRef;

            using (var docwriter = new PdfDocumentWriter(stream, options))
            {
                // Call OnDocumentBegin callback:
                if (OnDocumentBegin != null)
                    OnDocumentBegin(docwriter, document);

                // Dispatch all child elements:
                foreach (var childItem in document.Items)
                    this.Dispatch(childItem, docwriter, context);

                // Call OnDocumentEnd callback:
                if (OnDocumentEnd != null)
                    OnDocumentEnd(docwriter, document);
            }
        }

        /// <summary>
        /// Writes the given document item to the given document writer.
        /// </summary>
        public void Write(IDocumentItem documentItem, PdfDocumentWriter writer)
        {
            var context = new Context();
            this.Dispatch(documentItem, writer, context);
        }

        /// <summary>
        /// Writes a page item to the given PDF page writer.
        /// </summary>
        public void Write(IPageItem pageItem, PdfPageWriter writer)
        {
            var context = new Context();
            context.Coordinates = GetCoordinates(writer);
            this.Dispatch(pageItem, writer, context);
        }

        protected void Dispatch(IDocumentItem documentItem, PdfDocumentWriter writer, Context context)
        {
            if (!documentItem.Hidden)
                this.GetType().GetMethod("Handle" + documentItem.GetType().Name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, new object[] { documentItem, writer, context });
        }

        protected void Dispatch(IPageItem pageItem, PdfPageWriter writer, Context context)
        {
            if (!pageItem.Hidden)
                this.GetType().GetMethod("Handle" + pageItem.GetType().Name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, new object[] { pageItem, writer, context });
        }

        protected void HandlePage(Page item, PdfDocumentWriter writer, Context context)
        {
            var format = PdfPageFormat.A4Portrait;

            if (!String.IsNullOrWhiteSpace(item.Format))
                format = (PdfPageFormat)typeof(PdfPageFormat).GetField(item.Format).GetValue(null);
            if (!String.IsNullOrWhiteSpace(item.Width) && !String.IsNullOrWhiteSpace(item.Height))
                format = new PdfPageFormat(GetValue(item.Width, format.Width), GetValue(item.Height, format.Height));

            var pageWriter = writer.NewPage(format);

            context = new Context(context);
            context.Coordinates = GetCoordinates(pageWriter);
            context.GraphicsOptionsRef = item.GraphicsOptionsRef ?? context.GraphicsOptionsRef;
            context.TextOptionsRef = item.TextOptionsRef ?? context.TextOptionsRef;

            if (item.CoordinateSpace != null)
            {
                context.Coordinates = new Coordinates(context.Coordinates.Physical, 0.0, 0.0, item.CoordinateSpace.Width, item.CoordinateSpace.Height);
            }

            try
            {
                // Call OnPageBegin callback:
                if (OnPageBegin != null)
                    OnPageBegin(pageWriter, item);

                // Dispatch page items:
                foreach (var pageItem in item.Items)
                    this.Dispatch(pageItem, pageWriter, context);

                // Call OnPageEnd callback:
                if (OnPageEnd != null)
                    OnPageEnd(pageWriter, item);
            }
            finally
            {
                pageWriter.Dispose();
            }
        }

        protected void HandleArea(Area item, PdfPageWriter writer, Context context)
        {
            var logicalBox = ToLogicalBox(item, context);
            var physicalBox = ToPhysicalBox(logicalBox, context);

            context = new Context(context);
            context.GraphicsOptionsRef = item.GraphicsOptionsRef ?? context.GraphicsOptionsRef;
            context.TextOptionsRef = item.TextOptionsRef ?? context.TextOptionsRef;
            if (item.CoordinateSpace == null)
            {
                context.Coordinates = new Coordinates(physicalBox, logicalBox);
            }
            else
            {
                context.Coordinates = new Coordinates(physicalBox, 0.0, 0.0, item.CoordinateSpace.Width, item.CoordinateSpace.Height);
            }

            foreach (var subitem in item.Items)
                this.Dispatch(subitem, writer, context);
        }

        protected void HandleCross(Cross item, PdfPageWriter writer, Context context)
        {
            var physicalBox = ToPhysicalBox(item, context);
            var go = (PdfGraphicsOptions)GetReferenceObject(item.GraphicsOptionsRef ?? context.GraphicsOptionsRef) ?? new PdfGraphicsOptions();

            writer.DrawLine(physicalBox[0], physicalBox[1], physicalBox[2], physicalBox[3], go);
            writer.DrawLine(physicalBox[2], physicalBox[1], physicalBox[0], physicalBox[3], go);
        }

        protected void HandleFont(Font item, PdfDocumentWriter writer, Context context)
        {
            var font = (PdfPredefinedFont)typeof(PdfPredefinedFont).GetField(item.FontRef).GetValue(null);

            writer.RegisterFont(font);
        }

        protected void HandleGraphicsOptions(GraphicsOptions item, PdfDocumentWriter writer, Context context)
        {
            PdfGraphicsOptions go;
            if (!String.IsNullOrWhiteSpace(item.TemplateRef))
            {
                go = new PdfGraphicsOptions((PdfGraphicsOptions)GetReferenceObject(item.TemplateRef));
            }
            else
            {
                go = new PdfGraphicsOptions();
            }

            if (!String.IsNullOrWhiteSpace(item.LineCapStyle))
                go.LineCapstyle = (PdfLineCapStyle)Enum.Parse(typeof(PdfLineCapStyle), item.LineCapStyle);

            if (!String.IsNullOrWhiteSpace(item.LineDashPattern))
                go.LineDashPattern = (PdfLineDashPattern)typeof(PdfLineDashPattern).GetField(item.LineDashPattern).GetValue(null);

            if (!String.IsNullOrWhiteSpace(item.LineJoinStyle))
                go.LineJoinStyle = (PdfLineJoinStyle)Enum.Parse(typeof(PdfLineJoinStyle), item.LineJoinStyle);

            if (!String.IsNullOrWhiteSpace(item.StrokeColor))
                go.StrokeColor = (PdfColor)typeof(PdfColor).GetField(item.StrokeColor).GetValue(null);

            if (!String.IsNullOrWhiteSpace(item.StrokeWidth))
                go.StrokeWidth = Double.Parse(item.StrokeWidth ?? "1", CultureInfo.InvariantCulture);

            if (!String.IsNullOrWhiteSpace(item.FillColor))
                go.FillColor = (PdfColor)typeof(PdfColor).GetField(item.FillColor).GetValue(null);

            SetReferenceObject(item.Id, go);
        }

        protected void HandleImage(Image item, PdfPageWriter writer, Context context)
        {
            var physicalBox = ToPhysicalBox(item, context);
            //var x = physicalBox[0];
            //var y = physicalBox[1];
            var x = Math.Min(physicalBox[0], physicalBox[2]);
            var y = Math.Min(physicalBox[1], physicalBox[3]);
            var width = Math.Abs(physicalBox[2] - physicalBox[0]);
            var height = Math.Abs(physicalBox[3] - physicalBox[1]);
            var placement = (PdfImagePlacement)Enum.Parse(typeof(PdfImagePlacement), item.Placement ?? "Stretch");
            var rotation = GetAngleValue(item.Rotation);
            var imageRotation = (PdfImageRotation)Enum.Parse(typeof(PdfImageRotation), item.ImageRotation ?? "None");

            if (String.IsNullOrWhiteSpace(item.ImageRef))
            {
                item.ImageRef = item.FileName ?? item.Url ?? Guid.NewGuid().ToString();
                if (!this.referables.ContainsKey(item.ImageRef))
                {
                    var img = LoadImage(item.FileName, item.Url);
                    var pdfRef = writer.DocumentWriter.AddImage(img);
                    SetReferenceObject(item.ImageRef, pdfRef);
                }
            }

            if (rotation != 0.0)
            {
                if (!String.IsNullOrWhiteSpace(item.ImageRef))
                {
                    var imgRef = (PdfObjectRef)GetReferenceObject(item.ImageRef);
                    writer.DrawImageRef(x, y, imgRef, width, rotation);
                }
                else
                {
                    var img = new System.Drawing.Bitmap(item.FileName ?? item.Url);
                    writer.DrawImage(x, y, img, width, rotation);
                }
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(item.ImageRef))
                {
                    var imgRef = (PdfObjectRef)GetReferenceObject(item.ImageRef);
                    writer.DrawImageRef(x, y, imgRef, width, height, placement, imageRotation);
                }
                else
                {
                    var img = new System.Drawing.Bitmap(item.FileName ?? item.Url);
                    writer.DrawImage(x, y, img, width, height, placement, imageRotation);
                }
            }
        }

        protected void HandleImageObject(ImageObject item, PdfDocumentWriter writer, Context context)
        {
            item.Id = item.Id ?? item.FileName ?? item.Url;

            if (!this.referables.ContainsKey(item.Id))
            {
                var img = LoadImage(item.FileName, item.Url);
                var pdfRef = writer.AddImage(img);
                SetReferenceObject(item.Id, pdfRef);
            }
        }

        protected void HandleLine(Line item, PdfPageWriter writer, Context context)
        {
            var physicalBox = ToPhysicalBox(item, context);
            var go = (PdfGraphicsOptions)GetReferenceObject(item.GraphicsOptionsRef ?? context.GraphicsOptionsRef) ?? new PdfGraphicsOptions();

            writer.DrawLine(physicalBox[0], physicalBox[1], physicalBox[2], physicalBox[3], go);
        }

        protected void HandleOval(Oval item, PdfPageWriter writer, Context context)
        {
            var physicalBox = ToPhysicalBox(item, context);

            var go = (PdfGraphicsOptions)GetReferenceObject(item.GraphicsOptionsRef ?? context.GraphicsOptionsRef) ?? new PdfGraphicsOptions();

            writer.DrawOval2(physicalBox[0], physicalBox[1], physicalBox[2], physicalBox[3], go);
        }

        protected void HandleRectangle(Rectangle item, PdfPageWriter writer, Context context)
        {
            var physicalBox = ToPhysicalBox(item, context);
            var rotation = GetAngleValue(item.Rotation);
            var radius = Double.Parse(item.Radius ?? "0.0", CultureInfo.InvariantCulture);

            var go = (PdfGraphicsOptions)GetReferenceObject(item.GraphicsOptionsRef ?? context.GraphicsOptionsRef) ?? new PdfGraphicsOptions();

            if (radius > 0.0 && rotation == 0.0)
            {
                writer.DrawRoundedRectangle2(Math.Min(physicalBox[0], physicalBox[2]), Math.Min(physicalBox[1], physicalBox[3]), Math.Max(physicalBox[2], physicalBox[0]), Math.Max(physicalBox[3], physicalBox[1]), radius, go);
            }
            else
            {
                writer.DrawRectangle2(physicalBox[0], physicalBox[1], physicalBox[2], physicalBox[3], go, rotation);
            }
        }

        protected void HandleScript(Script item, PdfPageWriter writer, Context context)
        {
            if (!String.IsNullOrWhiteSpace(item.ScriptRef))
            {
                writer.WriteObjectRef((PdfObjectRef)GetReferenceObject(item.ScriptRef));
            }
            else
            {
                var so = new PdfScriptObject();
                so.Write(item.Content);
                writer.WriteObject(so);
            }
        }

        protected void HandleScriptObject(ScriptObject item, PdfDocumentWriter writer, Context context)
        {
            if (!this.referables.ContainsKey(item.Id))
            {
                var so = new PdfScriptObject();
                so.Write(item.Content);
                var pdfRef = writer.WriteObject(so);
                SetReferenceObject(item.Id, pdfRef);
            }
        }

        protected void HandleText(Text item, PdfPageWriter writer, Context context)
        {
            var physicalBox = ToPhysicalPoint(item, context);
            var x = physicalBox[0];
            var y = physicalBox[1];
            var to = (PdfTextOptions)GetReferenceObject(item.TextOptionsRef ?? context.TextOptionsRef) ?? new PdfTextOptions();
            var leftRotationDegrees = GetAngleValue(item.Rotation);

            // Correct y value to reflect top of text:
            y = y - to.FontSize;

            var content = item.Content;
            if (!String.IsNullOrWhiteSpace(item.ContentKey))
            {
                if (this.ContentSource == null) throw new NullReferenceException("Failed to render Text content using ContentKey as ContentSource of PdfModelWriter is not set.");
                content = this.ContentSource[item.ContentKey] ?? item.Content ?? String.Empty;
            }

            writer.DrawText(x, y, content ?? String.Empty, to, leftRotationDegrees);
        }

        protected void HandleTextBlock(TextBlock item, PdfPageWriter writer, Context context)
        {
            var physicalBox = ToPhysicalBox(item, context);
            var x = physicalBox[0];
            var y = physicalBox[1];
            var width = physicalBox[2] - physicalBox[0];
            var height = physicalBox[3] - physicalBox[1];
            var to = (PdfTextOptions)GetReferenceObject(item.TextOptionsRef ?? context.TextOptionsRef) ?? new PdfTextOptions();
            var leftRotationDegrees = GetAngleValue(item.Rotation);
            var alignment = Int32.Parse(item.Alignment.CaseTranslate(StringComparison.OrdinalIgnoreCase, "", "-1", "left", "-1", "center", "0", "middle", "0", "right", "1", item.Alignment) ?? "-1");

            // Correct y value to reflect top of textblock:
            y = y - to.FontSize;

            var content = item.Content;
            if (!String.IsNullOrWhiteSpace(item.ContentKey))
            {
                if (this.ContentSource == null) throw new NullReferenceException("Failed to render TextBlock content using ContentKey as ContentSource of PdfModelWriter is not set.");
                content = this.ContentSource[item.ContentKey] ?? item.Content ?? String.Empty;
            }

            writer.DrawTextblock(x, y, content ?? String.Empty, width, to, leftRotationDegrees, alignment);
        }

        protected void HandleTextOptions(TextOptions item, PdfDocumentWriter writer, Context context)
        {
            PdfTextOptions to;
            if (!String.IsNullOrWhiteSpace(item.TemplateRef))
            {
                to = new PdfTextOptions((PdfTextOptions)GetReferenceObject(item.TemplateRef));
            }
            else
            {
                to = new PdfTextOptions();
            }

            if (!String.IsNullOrWhiteSpace(item.FontRef))
                to.Font = (PdfPredefinedFont)typeof(PdfPredefinedFont).GetField(item.FontRef).GetValue(null);

            if (!String.IsNullOrWhiteSpace(item.FontSize))
                to.FontSize = this.GetValue(item.FontSize, 100);

            if (!String.IsNullOrWhiteSpace(item.InkColor))
                to.InkColor = (PdfColor)typeof(PdfColor).GetField(item.InkColor).GetValue(null);

            if (!String.IsNullOrWhiteSpace(item.LineCapStyle))
                to.LineCapStyle = (PdfLineCapStyle)Enum.Parse(typeof(PdfLineCapStyle), item.LineCapStyle);

            if (!String.IsNullOrWhiteSpace(item.LineDashPattern))
                to.LineDashPattern = (PdfLineDashPattern)typeof(PdfLineDashPattern).GetField(item.LineDashPattern).GetValue(null);

            if (!String.IsNullOrWhiteSpace(item.OutlineColor))
                to.OutlineColor = (PdfColor)typeof(PdfColor).GetField(item.OutlineColor).GetValue(null);

            if (!String.IsNullOrWhiteSpace(item.OutlineWidth))
                to.OutlineWidth = GetValue(item.OutlineWidth, 100);

            if (!String.IsNullOrWhiteSpace(item.RenderingMode))
                to.RenderingMode = (PdfTextRenderingMode)Enum.Parse(typeof(PdfTextRenderingMode), item.RenderingMode);

            SetReferenceObject(item.Id, to);
        }

        public object GetReferenceObject(string id)
        {
            if (id == null)
                return null;
            else
                return referables[id];
        }

        public void SetReferenceObject(string id, object obj)
        {
            referables[id] = obj;
        }

        #region Coordinates

        private Coordinates GetCoordinates(PdfPageWriter pageWriter)
        {
            return new Coordinates(0.0, pageWriter.Height, pageWriter.Width, 0.0);
        }

        private double[] ToLogicalBox(BoxItem item, Context context)
        {
            var x1 = GetValue(item.X ?? "0", context.Coordinates.LogicalWidth) + context.Coordinates.Logical[0];
            var y1 = GetValue(item.Y ?? "0", context.Coordinates.LogicalHeight) + context.Coordinates.Logical[1];
            var x2 = x1 + GetValue(item.Width ?? "100%", context.Coordinates.LogicalWidth);
            var y2 = y1 + GetValue(item.Height ?? "100%", context.Coordinates.LogicalHeight);
            return new double[] { x1, y1, x2, y2 };
        }

        private double[] ToPhysicalBox(double[] logicalBox, Context context)
        {
            var phxy1 = context.Coordinates.Translate(logicalBox[0], logicalBox[1]);
            var phxy2 = context.Coordinates.Translate(logicalBox[2], logicalBox[3]);
            return new double[] { phxy1[0], phxy1[1], phxy2[0], phxy2[1] };
        }

        private double[] ToPhysicalBox(BoxItem item, Context context)
        {
            return ToPhysicalBox(ToLogicalBox(item, context), context);
        }

        private double[] ToPhysicalPoint(PositionalItem item, Context context)
        {
            var x1 = GetValue(item.X ?? "0", context.Coordinates.LogicalWidth) + context.Coordinates.Logical[0];
            var y1 = GetValue(item.Y ?? "0", context.Coordinates.LogicalHeight) + context.Coordinates.Logical[1];
            var ph1 = context.Coordinates.Translate(x1, y1);
            return new double[] { ph1[0], ph1[1] };
        }

        private double GetValue(string rawValue, double ofFull)
        {
            var match = valuesRegex.Match(rawValue);
            var value = Double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            var unit = match.Groups[2].Value;

            if (String.IsNullOrWhiteSpace(unit))
            {
                return value;
            }
            else if ("%".Equals(unit))
            {
                return value / 100.0 * ofFull;
            }
            else if ("cm".Equals(unit))
            {
                return value * 28.3165;
            }
            else if ("mm".Equals(unit))
            {
                return value * 2.83165;
            }
            else
            {
                throw new ArgumentException(String.Format("Unit of value \"{0}\" not supported.", rawValue));
            }
        }

        private double GetAngleValue(string rawValue)
        {
            if (String.IsNullOrWhiteSpace(rawValue)) return 0.0;

            var match = valuesRegex.Match(rawValue);
            var value = Double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            var unit = match.Groups[2].Value;

            if (String.IsNullOrWhiteSpace(unit) || "°".Equals(unit) || "deg".Equals(unit))
            {
                return value;
            }
            else if ("%".Equals(unit))
            {
                return value / 100 * 360;
            }
            else if ("rad".Equals(unit))
            {
                return value / 2 / Math.PI * 360;
            }
            else if ("frac".Equals(unit))
            {
                return value * 360;
            }
            else
            {
                throw new ArgumentException(String.Format("Unit of value \"{0}\" not supported.", rawValue));
            }
        }

        protected class Context
        {
            public Context(Context previous)
            {
                this.Coordinates = previous.Coordinates;
                this.GraphicsOptionsRef = previous.GraphicsOptionsRef;
                this.TextOptionsRef = previous.TextOptionsRef;
            }

            public Context()
            { }

            public Coordinates Coordinates { get; set; }

            public string GraphicsOptionsRef { get; set; }

            public string TextOptionsRef { get; set; }
        }

        #endregion

        #region Helpers

        private System.Drawing.Image LoadImage(string filename, string url)
        {
            if (!String.IsNullOrWhiteSpace(url))
            {
                var client = new WebClient();
                using (var stream = client.OpenRead(url))
                {
                    return System.Drawing.Bitmap.FromStream(stream);
                }
            }
            else
            {
                return new System.Drawing.Bitmap(filename);
            }
        }

        #endregion
    }
}
