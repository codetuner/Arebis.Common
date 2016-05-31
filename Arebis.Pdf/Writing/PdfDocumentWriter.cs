using Arebis.Pdf.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Pdf.Writing
{
    /// <summary>
    /// A PdfDocumentWriter provides you a low-level access API for writing
    /// Adobe PDF documents.
    /// </summary>
    public class PdfDocumentWriter : IDisposable
    {
        public string PdfVersion = "1.4";
        public string PdfCreator = "Arebis.Pdf .NET Library";

        private int CurrentGenerationId = 0;

        private PdfObjectRef PagesRef;
        private PdfObjectRef ResourcesRef;
        private PdfObjectRef CatalogRef;
        private PdfObjectRef InfoRef;

        public PdfDocumentWriter(Stream stream, PdfDocumentOptions options = null)
        {
            // Initialize collections and object references:
            this.Xref = new List<long>();
            this.Fonts = new Dictionary<string, PdfObjectRef>();
            this.XObjects = new Dictionary<string, PdfObjectRef>();
            this.XObjectsRev = new Dictionary<PdfObjectRef, string>();
            this.ImageRatios = new Dictionary<PdfObjectRef, double>();
            this.CatalogRef = NewObjectRef();
            this.PagesRef = NewObjectRef();
            this.ResourcesRef = NewObjectRef();
            this.InfoRef = NewObjectRef();

            // Wrap the given stream in a Position-logging stream and build a StreamWriter:
            this.InnerWriter = new StreamWriter(new PositionStream(stream), Encoding.ASCII);

            // Make sure options are set:
            this.Options = options ?? new PdfDocumentOptions();

            // Write document introduction:
            this.WriteIntro();
        }

        public PdfDocumentOptions Options { get; private set; }

        protected List<long> Xref { get; private set; }

        protected Dictionary<string, PdfObjectRef> XObjects { get; private set; }

        protected Dictionary<PdfObjectRef, string> XObjectsRev { get; private set; }

        protected Dictionary<PdfObjectRef, double> ImageRatios { get; private set; }

        protected Dictionary<string, PdfObjectRef> Fonts { get; private set; }

        protected List<PdfObjectRef> PageRefs = new List<PdfObjectRef>();

        protected StreamWriter InnerWriter { get; private set; }

        /// <summary>
        /// Starts a new page. Dispose page when done.
        /// </summary>
        public virtual PdfPageWriter NewPage(PdfPageFormat format)
        {
            return new PdfPageWriter(this, format);
        }

        /// <summary>
        /// Starts a new page. Dispose page when done.
        /// </summary>
        public PdfPageWriter NewPage(double width, double height)
        {
            return NewPage(new PdfPageFormat(width, height));
        }

        /// <summary>
        /// Writes raw bytes to the PDF document stream.
        /// </summary>
        public void WriteRaw(byte[] bytes)
        {
            this.WriteRaw(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes raw bytes to the PDF document stream.
        /// </summary>
        public virtual void WriteRaw(byte[] bytes, int offset, int count)
        {
            this.InnerWriter.Flush();
            this.InnerWriter.BaseStream.Write(bytes, offset, count);
        }

        /// <summary>
        /// Writes raw string to the PDF document stream.
        /// </summary>
        public virtual void WriteRaw(string s)
        {
            this.InnerWriter.Write(s);
        }

        /// <summary>
        /// Flushes the document stream.
        /// </summary>
        public virtual void Flush()
        {
            if (this.InnerWriter != null) this.InnerWriter.Flush();
        }

        /// <summary>
        /// Gets the current byte position within the stream.
        /// </summary>
        protected long Position
        {
            get
            {
                this.Flush();
                return this.InnerWriter.BaseStream.Position;
            }
            //set
            //{
            //    this.Flush();
            //    this.InnerWriter.BaseStream.Seek(value, SeekOrigin.Begin);
            //}
        }

        /// <summary>
        /// Writes the PDF document trailer and closes the stream.
        /// </summary>
        public virtual void Close()
        {
            if (this.InnerWriter != null)
            {
                this.WriteTrailer();

                this.Flush();

                this.InnerWriter.Dispose();
                this.InnerWriter = null;
            }
        }

        protected virtual void WriteIntro()
        {
            var binarybytes = new byte[] { 0xE2, 0xE3, 0xCF, 0xD3 };
            WriteRaw("%PDF-" + PdfVersion + "\n%");
            WriteRaw(binarybytes, 0, binarybytes.Length);
            WriteRaw("\n%Arebis.Pdf .NET Library\n");

            var catalog = new PdfObject();
            catalog.Data["Type"] = "/Catalog";
            catalog.Data["Version"] = "/" + PdfVersion;
            catalog.Data["Pages"] = PagesRef;
            WriteObject(catalog, CatalogRef);

            var info = new PdfObject();
            if (!String.IsNullOrWhiteSpace(this.Options.Title)) info.Data["Title"] = '(' + this.Options.Title + ')';
            if (!String.IsNullOrWhiteSpace(this.Options.Subject)) info.Data["Subject"] = '(' + this.Options.Subject + ')';
            if (!String.IsNullOrWhiteSpace(this.Options.Keywords)) info.Data["Keywords"] = '(' + this.Options.Keywords + ')';
            info.Data["Author"] = '(' + ((!String.IsNullOrWhiteSpace(this.Options.Author)) ? this.Options.Author : Environment.UserName) + ')';
            info.Data["Creator"] = "(" + PdfCreator + ")";
            var now = DateTime.Now;
            var timezone = TimeZone.CurrentTimeZone.GetUtcOffset(now);
            info.Data["CreationDate"] = "(D:" + now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + (timezone.Ticks >= 0 ? '+' : '-') + String.Format("{0:00}'{1:00}'", Math.Abs(timezone.Hours), Math.Abs(timezone.Minutes)) + ")";
            WriteObject(info, InfoRef);
        }

        /// <summary>
        /// Adds an image to the PDF document and returns a reference object to it.
        /// </summary>
        public PdfObjectRef AddImage(Image image)
        {
            var obj = new PdfObject();
            obj.Data["Subtype"] = "/Image";
            obj.Data["Width"] = image.Width;
            obj.Data["Height"] = image.Height;
            obj.Data["BitsPerComponent"] = "8";
            obj.Data["ColorSpace"] = "/DeviceRGB";
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                obj.Stream = new PdfBinaryStream("/DCTDecode", ms.ToArray());
            }

            var objRef = AddXObject(obj);
            ImageRatios[objRef] = (double)image.Height / image.Width;
            return objRef;
        }

        /// <summary>
        /// Adds an 'XObject' to the stream and returns a reference object to it.
        /// </summary>
        public PdfObjectRef AddXObject(PdfObject obj)
        {
            var objRef = this.NewObjectRef();
            var name = (obj.Data.ContainsKey("Name")) ? obj.Data["Name"].ToString() : objRef.ToDefaultName();
            if (obj.Data.ContainsKey("Type") && !"/XObject".Equals(obj.Data["Type"])) throw new ArgumentException("PdfObject is not a valid XObject, it's Type must be '/XObject'.");
            obj.Data["Type"] = "/XObject";
            if (!obj.Data.ContainsKey("Subtype")) obj.Data["Subtype"] = "/Form";
            obj.Data["Name"] = name;

            this.WriteObject(obj, objRef);
            RegisterXObject(objRef, name);
            return objRef;
        }

        protected void RegisterXObject(PdfObjectRef objRef, string name)
        {
            XObjects[name] = objRef;
            XObjectsRev[objRef] = name;
        }

        /// <summary>
        /// Retrieves the name of a previously added XObject.
        /// </summary>
        public string GetNameOfXObject(PdfObjectRef objRef)
        {
            return this.XObjectsRev[objRef];
        }

        /// <summary>
        /// Returns the aspect ratio of a previously added image.
        /// </summary>
        [Obsolete("Use GetImageHeight() to compute the height if an image giving its width.")]
        public double GetImageAspectRatio(PdfObjectRef imageRef)
        {
            return this.ImageRatios[imageRef];
        }

        /// <summary>
        /// Returns the height the previoulsy added image should have, when rendered with the given width, in order to have
        /// the same aspect ration.
        /// </summary>
        /// <param name="imageRef">Reference to the image previoulsy added.</param>
        /// <param name="forWidth">Width to render the image with.</param>
        public double GetImageHeight(PdfObjectRef imageRef, double forWidth)
        {
            return this.ImageRatios[imageRef] * forWidth;
        }

        /// <summary>
        /// Registers a font for use in this PDF document.
        /// </summary>
        public PdfObjectRef RegisterFont(PdfFont font)
        {
            PdfObjectRef fontRef;
            if (this.Fonts.TryGetValue(font.Name, out fontRef))
            {
                return fontRef;
            }
            else
            {
                fontRef = this.Fonts[font.Name] = this.WriteObject(font);
                return fontRef;
            }
        }

        /// <summary>
        /// Creates a new PdfObjectRef object with the documents generation Id and next (unique) object Id.
        /// </summary>
        public virtual PdfObjectRef NewObjectRef()
        {
            Xref.Add(-1L);
            return new PdfObjectRef(CurrentGenerationId, Xref.Count);
        }

        /// <summary>
        /// Writes a PdfObject to this document and returns the reference to that object.
        /// Note that this does not add the object to a page.
        /// </summary>
        public PdfObjectRef WriteObject(PdfObject obj)
        {
            var objRef = NewObjectRef();
            this.WriteObject(obj, objRef);
            return objRef;
        }

        /// <summary>
        /// Writes a PdfObject to this document given its object reference.
        /// </summary>
        public virtual void WriteObject(PdfObject obj, PdfObjectRef objRef)
        {
            if (obj is PdfScriptObject)
            {
                foreach (var font in ((PdfScriptObject)obj).ReferencedFonts)
                    RegisterFont(font);
            }

            Xref[objRef.ObjectId-1] = this.Position;

            var builder = new StringBuilder();
            builder.Append(objRef.ObjectId + " " + objRef.GenerationId + " obj\n");
            if (obj.Data != null || obj.Stream != null)
            {
                builder.Append("<<\n");
                if (obj.Data != null)
                {
                    foreach (var pair in obj.Data)
                    {
                        builder.Append('/');
                        builder.Append(pair.Key);
                        builder.Append(' ');
                        builder.Append(pair.Value.ToString());
                        builder.Append('\n');
                    }
                }
                if (obj.Stream == null)
                {
                    builder.Append(">>\n");
                }
                else if (obj.Stream is PdfBinaryStream)
                {
                    var s = ((PdfBinaryStream)obj.Stream);
                    builder.Append("/Length ");
                    builder.Append(s.Length);
                    builder.Append("\n/Filter [");
                    builder.Append(s.Filter);
                    builder.Append("]\n>>\n");
                    builder.Append("stream\n");

                    this.WriteRaw(builder.ToString());
                    builder.Length = 0;
                    this.WriteRaw(s.Content, 0, s.Content.Length);

                    builder.Append("\nendstream\n");
                }
                else
                {
                    byte[] bytes;
                    if ((obj.Stream is PdfTextStream) && (this.Options.TextFilter != null))
                    {
                        bytes = this.Options.TextFilter.EncodeString(((PdfTextStream)obj.Stream).Content.ToString());
                        builder.Append("/Filter [");
                        builder.Append(this.Options.TextFilter.Name);
                        builder.Append("]\n");
                    }
                    else
                    {
                        bytes = Encoding.Default.GetBytes(((PdfTextStream)obj.Stream).Content.ToString());
                    }
                    builder.Append("/Length ");
                    builder.Append(bytes.Length);
                    builder.Append('\n');

                    builder.Append(">>\n");

                    builder.Append("stream\n");

                    this.WriteRaw(builder.ToString());
                    builder.Length = 0;
                    this.WriteRaw(bytes, 0, bytes.Length);

                    builder.Append("\nendstream\n");
                }
            }
            builder.Append("endobj\n");

            this.WriteRaw(builder.ToString());
        }

        /// <summary>
        /// Writes a PdfPageObject and links it to the pages tree.
        /// For internal use.
        /// </summary>
        public void WritePage(PdfObject pageObj)
        {
            pageObj.Data["Type"] = "/Page";
            pageObj.Data["Parent"] = PagesRef;
            pageObj.Data["Resources"] = ResourcesRef;
            var pageObjRef = this.WriteObject(pageObj);
            PageRefs.Add(pageObjRef);
        }

        protected virtual void WriteTrailer()
        {
            var resources = new PdfObject();
            if (this.Fonts.Count > 0)
                resources.Data["Font"] = "<< " + String.Join(" ", this.Fonts.Select(kv => kv.Key + " " + kv.Value)) + " >>";
            if (this.XObjects.Count > 0)
                resources.Data["XObject"] = "<< " + String.Join(" ", this.XObjects.Select(kv => kv.Key + " " + kv.Value)) + " >>";
            this.WriteObject(resources, ResourcesRef);

            var pages = new PdfObject();
            pages.Data["Type"] = "/Pages";
            pages.Data["Count"] = PageRefs.Count;
            pages.Data["Kids"] = "[" + String.Join(" ", PageRefs.Select(r => r.ToString())) + "]";
            WriteObject(pages, PagesRef);

            var xrefStart = this.Position;
            var builder = new StringBuilder();
            builder.Append("xref\r\n");
            builder.Append("0 ");
            builder.Append(Xref.Count + 1);
            builder.Append("\r\n");
            builder.Append("0000000000 65535 f\r\n");
            foreach (var offset in Xref)
            {
                if (offset < 0)
                {
                    builder.AppendFormat("{0:0000000000} {1:00000} {2}\r\n", 0L, CurrentGenerationId, 'f');
                }
                else
                {
                    builder.AppendFormat("{0:0000000000} {1:00000} {2}\r\n", offset, CurrentGenerationId, 'n');
                }
            }

            var fileId = Guid.NewGuid().ToString().Replace("-", "");
            builder.Append("trailer\n<<\n/Size ");
            builder.Append(Xref.Count + 1);
            builder.Append("\n/Root ");
            builder.Append(CatalogRef);
            builder.Append("\n/Info ");
            builder.Append(InfoRef);
            builder.Append("\n/ID [<");
            builder.Append(fileId);
            builder.Append("><");
            builder.Append(fileId);
            builder.Append(">]");
            builder.Append("\n>>\nstartxref\n");
            builder.Append(xrefStart);
            builder.Append("\n%%EOF\n");

            this.WriteRaw(builder.ToString());
        }

        /// <summary>
        /// Reserves room in the document for a digital signature.
        /// </summary>
        [Obsolete("Signing not yet supported.")]
        public virtual void WriteSignaturePlaceHolder()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Signs the document and closes the document stream.
        /// </summary>
        [Obsolete("Signing not yet supported.")]
        public virtual void SignAndClose(PdfSignatureInformation signatureInformation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes and disposes this document writer.
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }
    }
}
