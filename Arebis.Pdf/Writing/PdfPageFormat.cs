using System;

namespace Arebis.Pdf.Writing
{
    [Serializable]
    public class PdfPageFormat
    {
        /// <summary>
        /// Instantiates a new page format.
        /// Use predefined static instances for A3, A4, A5 and Letter.
        /// </summary>
        /// <param name="width">Width of the page in points (1 inch = 72 points).</param>
        /// <param name="height">Height of the page in points (1 inch = 72 points).</param>
        /// <param name="orientation">Page orientation.</param>
        public PdfPageFormat(double width, double height, PdfPageOrientation orientation = PdfPageOrientation.Portrait)
        {
            this.Height = height;
            this.Width = width;
            this.Orientation = orientation;
        }

        /// <summary>
        /// Height of the page in points (1 inch = 72 points).
        /// </summary>
        public double Height { get; private set; }

        /// <summary>
        /// Width of the page in points (1 inch = 72 points).
        /// </summary>
        public double Width { get; private set; }
        
        /// <summary>
        /// Page orientation.
        /// </summary>
        public PdfPageOrientation Orientation { get; private set; }

        /// <summary>
        /// A3 paper format in portrait (1190x1682).
        /// </summary>
        public static readonly PdfPageFormat A3Portrait = new PdfPageFormat(1190.0, 1682.0, PdfPageOrientation.Portrait);

        /// <summary>
        /// A3 paper format in landscape (1682x1190).
        /// </summary>
        public static readonly PdfPageFormat A3Landscape = new PdfPageFormat(1190.0, 1682.0, PdfPageOrientation.Landscape);

        /// <summary>
        /// A4 paper format in portrait (595x841).
        /// </summary>
        public static readonly PdfPageFormat A4Portrait = new PdfPageFormat(595.0, 841.0, PdfPageOrientation.Portrait);

        /// <summary>
        /// A4 paper format in landscape (841x595).
        /// </summary>
        public static readonly PdfPageFormat A4Landscape = new PdfPageFormat(595.0, 841.0, PdfPageOrientation.Landscape);

        /// <summary>
        /// A5 paper format in portrait (297½x420½).
        /// </summary>
        public static readonly PdfPageFormat A5Portrait = new PdfPageFormat(297.5, 420.5, PdfPageOrientation.Portrait);

        /// <summary>
        /// A5 paper format in landscape (420½x297½).
        /// </summary>
        public static readonly PdfPageFormat A5Landscape = new PdfPageFormat(297.5, 420.5, PdfPageOrientation.Landscape);

        /// <summary>
        /// Letter paper format in portrait (612x792).
        /// </summary>
        public static readonly PdfPageFormat LetterPortrait = new PdfPageFormat(612.0, 792, PdfPageOrientation.Portrait);

        /// <summary>
        /// Letter paper format in landscape (792x612).
        /// </summary>
        public static readonly PdfPageFormat LetterLandscape = new PdfPageFormat(612.0, 792, PdfPageOrientation.Landscape);
    }

    [Serializable]
    public enum PdfPageOrientation
    { 
        Portrait = 0,
        Landscape = 1,
    }
}
