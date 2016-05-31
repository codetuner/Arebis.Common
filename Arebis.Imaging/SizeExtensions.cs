using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Arebis.Imaging
{
    public static class SizeExtensions
    {
        public static Size Resize(this Size actualSize, Size newSize, ResizeMode resizeMode, OrientationMode orientationMode)
        {
            if (orientationMode == OrientationMode.RotateForBestFit)
            {
                if ((actualSize.Width > actualSize.Height) && !(newSize.Width > newSize.Height))
                    newSize = newSize.SwapWidthHeight();
                else if ((actualSize.Width < actualSize.Height) && !(newSize.Width < newSize.Height))
                    newSize = newSize.SwapWidthHeight();
            }

            var heightResizeFactor = newSize.Height / (double)actualSize.Height;
            var widthResizeFactor = newSize.Width / (double)actualSize.Width;

            if (resizeMode == ResizeMode.FitsInBox)
            {
                if (heightResizeFactor > widthResizeFactor)
                {
                    newSize = new Size(newSize.Width, (int)((double)actualSize.Height * widthResizeFactor));
                }
                else
                {
                    newSize = new Size((int)((double)actualSize.Width * heightResizeFactor), newSize.Height);
                }
            }
            else if (resizeMode == ResizeMode.BoxFitsIn || resizeMode == ResizeMode.BoxFitsInCropped)
            {
                if (heightResizeFactor < widthResizeFactor)
                {
                    newSize = new Size(newSize.Width, (int)((double)actualSize.Height * widthResizeFactor));
                }
                else
                {
                    newSize = new Size((int)((double)actualSize.Width * heightResizeFactor), newSize.Height);
                }
            }
            else if (resizeMode == ResizeMode.Stretch)
            {
                // Do nothing
            }
            else
            {
                throw new ArgumentException("Unsupported ResizeMode value.");
            }
            
            return newSize;
        }

        public static Size SwapWidthHeight(this Size size)
        {
            return new Size(size.Height, size.Width);
        }
    }
}
