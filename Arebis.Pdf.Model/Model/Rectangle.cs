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
    /// A rectangle element.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Rectangle : Figure
    {
        /// <summary>
        /// Rotation of the rectangle (i.e. "45", "45°", "45deg", "0.7854rad", "12.5%" and "0.125frac" are all equivalent).
        /// </summary>
        /// <remarks>
        /// Note that if the rotation is given, the radius parameter is ignored.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Rotation { get; set; }

        /// <summary>
        /// The radius of the rectangle (to form rounded rectangles).
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Radius { get; set; }
    }
}
