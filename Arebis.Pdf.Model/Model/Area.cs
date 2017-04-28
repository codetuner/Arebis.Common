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
    /// An area of a PDF page that can have its own coordinate space and default options.
    /// </summary>
    [DataContract]
    [KnownType(typeof(PositionalItem))]
    [ContentProperty("Items")]
    [Serializable]
    public class Area : BoxItem, IIContainerItem<IPageItem>
    {
        /// <summary>
        /// Creates a new Area element.
        /// </summary>
        public Area()
        {
            this.Items = new List<IPageItem>();
        }

        /// <summary>
        /// The items in the area.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public IList<IPageItem> Items { get; private set; }

        /// <summary>
        /// A custom coordinate space for the area.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public CoordinateSpace CoordinateSpace { get; set; }

        /// <summary>
        /// Graphics options to use by default within the scope of this area.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string GraphicsOptionsRef { get; set; }

        /// <summary>
        /// Text options to use by default within the scope of this area.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string TextOptionsRef { get; set; }
    }
}
