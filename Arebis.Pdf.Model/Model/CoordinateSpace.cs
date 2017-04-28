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
    /// A custom coordinate space.
    /// </summary>
    [DataContract]
    [ContentProperty("Items")]
    [Serializable]
    public class CoordinateSpace
    {
        /// <summary>
        /// Width of the element expressed in the custom coordinate space.
        /// If missing, the width is expressed in the same unit as the underlying physical coordinate space.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = true)]
        public double? Width { get; set; }

        /// <summary>
        /// Height of the element expressed in the custom coordinate space.
        /// If missing, calculates the height based on the width, conserving the aspect ratio.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public double? Height { get; set; }
    }
}
