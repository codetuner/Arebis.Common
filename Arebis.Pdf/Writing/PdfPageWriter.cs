using Arebis.Pdf.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arebis.Pdf.Writing
{
    /// <summary>
    /// A PdfPageWriter helps in managing page objects and writing content to them.
    /// </summary>
    public class PdfPageWriter : IDisposable
    {
        internal protected PdfPageWriter(PdfDocumentWriter writer, PdfPageFormat format)
        {
            this.DocumentWriter = writer;
            this.Content = new List<PdfObjectRef>();
            if (format.Orientation == PdfPageOrientation.Portrait)
            {
                this.Height = format.Height;
                this.Width = format.Width;
            }
            else
            {
                this.Height = format.Width;
                this.Width = format.Height;
            }
        }

        /// <summary>
        /// Writer of the document this pagewriter belongs to.
        /// </summary>
        public PdfDocumentWriter DocumentWriter { get; private set; }

        /// <summary>
        /// Width of the page.
        /// </summary>
        public double Width { get; private set; }

        /// <summary>
        /// Height of the page.
        /// </summary>
        public double Height { get; private set; }

        /// <summary>
        /// Content of the page.
        /// </summary>
        public List<PdfObjectRef> Content { get; private set; }

        /// <summary>
        /// Writes the given object to the page (writes it on the document and adds it on this page).
        /// Returns an object reference to the object.
        /// </summary>
        public virtual PdfObjectRef WriteObject(PdfObject obj)
        {
            if (obj is PdfScriptObject)
            {
                foreach (var font in ((PdfScriptObject)obj).ReferencedFonts)
                    DocumentWriter.RegisterFont(font);
            }
            var objRef = DocumentWriter.WriteObject(obj);
            this.WriteObjectRef(objRef);
            return objRef;
        }

        /// <summary>
        /// Adds the object referenced by the given object reference to this page.
        /// </summary>
        public virtual void WriteObjectRef(PdfObjectRef objRef)
        {
            Content.Add(objRef);
        }

        /// <summary>
        /// Closes and disposes this PageWriter.
        /// </summary>
        public virtual void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Closes this PageWriter, writing as PageObject to the document.
        /// </summary>
        public virtual void Close()
        {
            if(DocumentWriter != null)
            {
                // Create and write the PageObject:
                var pageObj = new PdfObject();
                pageObj.Data["Type"] = "/Page";
                pageObj.Data["MediaBox"] = String.Format("[0 0 {0:0.###} {1:0.###}]", this.Width, this.Height);
                if (this.Content.Count > 0)
                    pageObj.Data["Contents"] = "[" + String.Join(" ", this.Content.Select(c => c.ToString())) + "]";
                DocumentWriter.WritePage(pageObj);

                // Page is now closed:
                DocumentWriter = null;
            }
        }
    }
}
