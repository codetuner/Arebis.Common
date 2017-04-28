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
    /// A page in a PDF document.
    /// </summary>
    [DataContract]
    [KnownType(typeof(PositionalItem))]
    [KnownType(typeof(Script))]
    [ContentProperty("Items")]
    [Serializable]
    public class Page : ModelItem, IDocumentItem, IIContainerItem<IPageItem>
    {
        /// <summary>
        /// Creates a new page in a PDF document.
        /// </summary>
        public Page()
        {
            this.Items = new List<IPageItem>();
        }

        /// <summary>
        /// Format of the page ("A3", "A4", "A5", "Letter").
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Format { get; set; }

        /// <summary>
        /// Custom height of the page (if not using the Format).
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Height { get; set; }

        /// <summary>
        /// Custom width of the page (if not using the Format).
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Width { get; set; }

        /// <summary>
        /// Items on this page.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public IList<IPageItem> Items { get; private set; }

        /// <summary>
        /// Coordinate space to use on this page.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public CoordinateSpace CoordinateSpace { get; set; }

        /// <summary>
        /// Graphics options to use by default within the scope of this page.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string GraphicsOptionsRef { get; set; }

        /// <summary>
        /// Text options to use by default within the scope of this page.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string TextOptionsRef { get; set; }
    }
}
