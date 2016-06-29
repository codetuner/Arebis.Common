using System;

namespace Arebis.Pdf.Writing
{
    [Serializable]
    public enum PdfImagePlacement
    {
        Stretch = 0,
        Center = 1,
        LeftOrTop = 2,
        RightOrBottom = 3,
        LeftOrBottom = 4,
        RightOrTop = 5,
    }

    public static class PdfImagePlacementExtensions
    {
        public static PdfImagePlacement Rotated(this PdfImagePlacement me, PdfImageRotation rotation)
        {
            for (var rotations = 0; rotations < (int)rotation; rotations++)
            {
                switch (me)
                {
                    case PdfImagePlacement.LeftOrTop:
                        me = PdfImagePlacement.RightOrTop;
                        break;
                    case PdfImagePlacement.RightOrTop:
                        me = PdfImagePlacement.RightOrBottom;
                        break;
                    case PdfImagePlacement.RightOrBottom:
                        me = PdfImagePlacement.LeftOrBottom;
                        break;
                    case PdfImagePlacement.LeftOrBottom:
                        me = PdfImagePlacement.LeftOrTop;
                        break;
                }
            }

            return me;
        }
    }
}
