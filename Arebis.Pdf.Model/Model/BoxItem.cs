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
    /// Base class for boxes page elements.
    /// </summary>
    [DataContract]
    [KnownType(typeof(Area))]
    [KnownType(typeof(Figure))]
    [KnownType(typeof(Image))]
    [KnownType(typeof(TextBlock))]
    [Serializable]
    public abstract class BoxItem : PositionalItem
    {
        /// <summary>
        /// Height of the element.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Height { get; set; }

        /// <summary>
        /// Width of the element.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Width { get; set; }
    }
}
