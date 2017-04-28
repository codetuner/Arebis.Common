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
    /// An image element.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Image : BoxItem
    {
        /// <summary>
        /// Reference to a definied ImageObj element.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string ImageRef { get; set; }

        /// <summary>
        /// URL of the image.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Url { get; set; }

        /// <summary>
        /// Filename of the image.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string FileName { get; set; }

        /// <summary>
        /// Placement or positioning of the image (Stretch, Center, LeftOrTop, RightOrBottom, LeftOrBottom, RightOrTop).
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Placement { get; set; }

        /// <summary>
        /// Free rotation of the image (i.e. "45", "45°", "45deg", "0.7854rad", "12.5%" and "0.125frac" are all equivalent).
        /// </summary>
        /// <remarks>
        /// If a free rotation is given, the image height, placement and imagerotation settings are ignorded.
        /// The image is drawn on it's X,Y location with given width and matching height and requested rotation.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Rotation { get; set; }

        /// <summary>
        /// Rotation of the image (None, Left, Right, UpsideDown).
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string ImageRotation { get; set; }
    }
}
