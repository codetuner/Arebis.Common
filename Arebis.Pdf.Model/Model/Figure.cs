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
    /// Base class for figurative PDF page elements.
    /// </summary>
    [DataContract]
    [KnownType(typeof(Cross))]
    [KnownType(typeof(Line))]
    [KnownType(typeof(Oval))]
    [KnownType(typeof(Rectangle))]
    [Serializable]
    public abstract class Figure : BoxItem
    {
        /// <summary>
        /// Graphics options to apply to the figure.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string GraphicsOptionsRef { get; set; }
    }
}
