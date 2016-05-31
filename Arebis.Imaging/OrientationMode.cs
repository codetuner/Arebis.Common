using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Arebis.Imaging
{
    /// <summary>
    /// Describes how to handle orienation.
    /// </summary>
    public enum OrientationMode
    {
        /// <summary>
        /// Keep orientation as it is.
        /// </summary>
        KeepAsIs = 0,

        /// <summary>
        /// Rotate if needed for best fit.
        /// </summary>
        RotateForBestFit = 1,
    }
}
