using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Arebis.Imaging
{
    /// <summary>
    /// Describes how the resize the image.
    /// </summary>
    public enum ResizeMode
    {
        /// <summary>
        /// Resize the image such that it fits in the box without being distorted.
        /// Image is either in width or in hight same size as box while the other
        /// dimension is equal or smaller.
        /// </summary>
        FitsInBox = 0,

        /// <summary>
        /// Resize the image such that the box fits inside the image. Image is either
        /// in width or in hight same size as box while the other dimension is equal
        /// or larger.
        /// </summary>
        BoxFitsIn = 1,

        /// <summary>
        /// Same as BoxFitsIn, except that the parts exceeding the box are cropped
        /// away.
        /// </summary>
        BoxFitsInCropped = 2,

        /// <summary>
        /// Stretch, distort image to fit in box.
        /// </summary>
        Stretch = 3,

        /// <summary>
        /// Crop the image to fit in the box.
        /// </summary>
        Crop = 4,

        /// <summary>
        /// Same number of (mega)pixels.
        /// </summary>
        SameSize = 5,
    }
}
