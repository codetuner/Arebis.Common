using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Pdf.Model
{
    /// <summary>
    /// Base class for positional page elements.
    /// </summary>
    [DataContract]
    [KnownType(typeof(BoxItem))]
    [KnownType(typeof(Text))]
    [Serializable]
    public abstract class PositionalItem : ModelItem, IPageItem
    {
        /// <summary>
        /// X coordinate.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string X { get; set; }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Y { get; set; }
    }
}
