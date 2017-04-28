using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Arebis.Pdf.Model
{
    /// <summary>
    /// Represents a PDF document.
    /// </summary>
    [DataContract]
    [KnownType(typeof(Page))]
    [KnownType(typeof(ModelItem))]
    [KnownType(typeof(Font))]
    [ContentProperty("Items")]
    [Serializable]
    public class Document : ModelItem, IIContainerItem<IDocumentItem>
    {
        /// <summary>
        /// Constructs a new PDF document.
        /// </summary>
        public Document()
        {
            this.Items = new List<IDocumentItem>();
        }

        /// <summary>
        /// Title of the document.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Title { get; set; }

        /// <summary>
        /// Subject of the document.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Subject { get; set; }

        /// <summary>
        /// Keywords of the document.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Keywords { get; set; }

        /// <summary>
        /// Author of the document.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Author { get; set; }

        /// <summary>
        /// Content of the document.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public IList<IDocumentItem> Items { get; private set; }

        /// <summary>
        /// Graphics options to use by default within the scope of the document.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string GraphicsOptionsRef { get; set; }

        /// <summary>
        /// Text options to use by default within the scope of the document.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string TextOptionsRef { get; set; }
    }
}
