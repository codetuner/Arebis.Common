using System;
using System.Globalization;

namespace Arebis.Pdf.Common
{
    [Serializable]
    public class PdfColor
    {
        private string str;

        public PdfColor(byte red, byte green, byte blue, byte alpha = 255)
        {
            this.R = red;
            this.G = green;
            this.B = blue;
            this.A = alpha;
            str = String.Format(CultureInfo.InvariantCulture, "{0:0.###} {1:0.###} {2:0.###}", red / 255.0, green / 255.0, blue / 255.0);
        }

        public byte R { get; private set; }

        public byte G { get; private set; }
        
        public byte B { get; private set; }
        
        public byte A { get; private set; }

        public override string ToString()
        {
            return str;
        }

        //public static PdfColor Transparent = new PdfColor(255, 255, 255, 0);
        public static PdfColor AliceBlue = new PdfColor(240, 248, 255, 255);
        public static PdfColor AntiqueWhite = new PdfColor(250, 235, 215, 255);
        public static PdfColor Aqua = new PdfColor(0, 255, 255, 255);
        public static PdfColor Aquamarine = new PdfColor(127, 255, 212, 255);
        public static PdfColor Azure = new PdfColor(240, 255, 255, 255);
        public static PdfColor Beige = new PdfColor(245, 245, 220, 255);
        public static PdfColor Bisque = new PdfColor(255, 228, 196, 255);
        public static PdfColor Black = new PdfColor(0, 0, 0, 255);
        public static PdfColor BlanchedAlmond = new PdfColor(255, 235, 205, 255);
        public static PdfColor Blue = new PdfColor(0, 0, 255, 255);
        public static PdfColor BlueViolet = new PdfColor(138, 43, 226, 255);
        public static PdfColor Brown = new PdfColor(165, 42, 42, 255);
        public static PdfColor BurlyWood = new PdfColor(222, 184, 135, 255);
        public static PdfColor CadetBlue = new PdfColor(95, 158, 160, 255);
        public static PdfColor Chartreuse = new PdfColor(127, 255, 0, 255);
        public static PdfColor Chocolate = new PdfColor(210, 105, 30, 255);
        public static PdfColor Coral = new PdfColor(255, 127, 80, 255);
        public static PdfColor CornflowerBlue = new PdfColor(100, 149, 237, 255);
        public static PdfColor Cornsilk = new PdfColor(255, 248, 220, 255);
        public static PdfColor Crimson = new PdfColor(220, 20, 60, 255);
        public static PdfColor Cyan = new PdfColor(0, 255, 255, 255);
        public static PdfColor DarkBlue = new PdfColor(0, 0, 139, 255);
        public static PdfColor DarkCyan = new PdfColor(0, 139, 139, 255);
        public static PdfColor DarkGoldenrod = new PdfColor(184, 134, 11, 255);
        public static PdfColor DarkGray = new PdfColor(169, 169, 169, 255);
        public static PdfColor DarkGreen = new PdfColor(0, 100, 0, 255);
        public static PdfColor DarkKhaki = new PdfColor(189, 183, 107, 255);
        public static PdfColor DarkMagenta = new PdfColor(139, 0, 139, 255);
        public static PdfColor DarkOliveGreen = new PdfColor(85, 107, 47, 255);
        public static PdfColor DarkOrange = new PdfColor(255, 140, 0, 255);
        public static PdfColor DarkOrchid = new PdfColor(153, 50, 204, 255);
        public static PdfColor DarkRed = new PdfColor(139, 0, 0, 255);
        public static PdfColor DarkSalmon = new PdfColor(233, 150, 122, 255);
        public static PdfColor DarkSeaGreen = new PdfColor(143, 188, 139, 255);
        public static PdfColor DarkSlateBlue = new PdfColor(72, 61, 139, 255);
        public static PdfColor DarkSlateGray = new PdfColor(47, 79, 79, 255);
        public static PdfColor DarkTurquoise = new PdfColor(0, 206, 209, 255);
        public static PdfColor DarkViolet = new PdfColor(148, 0, 211, 255);
        public static PdfColor DeepPink = new PdfColor(255, 20, 147, 255);
        public static PdfColor DeepSkyBlue = new PdfColor(0, 191, 255, 255);
        public static PdfColor DimGray = new PdfColor(105, 105, 105, 255);
        public static PdfColor DodgerBlue = new PdfColor(30, 144, 255, 255);
        public static PdfColor Firebrick = new PdfColor(178, 34, 34, 255);
        public static PdfColor FloralWhite = new PdfColor(255, 250, 240, 255);
        public static PdfColor ForestGreen = new PdfColor(34, 139, 34, 255);
        public static PdfColor Fuchsia = new PdfColor(255, 0, 255, 255);
        public static PdfColor Gainsboro = new PdfColor(220, 220, 220, 255);
        public static PdfColor GhostWhite = new PdfColor(248, 248, 255, 255);
        public static PdfColor Gold = new PdfColor(255, 215, 0, 255);
        public static PdfColor Goldenrod = new PdfColor(218, 165, 32, 255);
        public static PdfColor Gray = new PdfColor(128, 128, 128, 255);
        public static PdfColor Green = new PdfColor(0, 128, 0, 255);
        public static PdfColor GreenYellow = new PdfColor(173, 255, 47, 255);
        public static PdfColor Honeydew = new PdfColor(240, 255, 240, 255);
        public static PdfColor HotPink = new PdfColor(255, 105, 180, 255);
        public static PdfColor IndianRed = new PdfColor(205, 92, 92, 255);
        public static PdfColor Indigo = new PdfColor(75, 0, 130, 255);
        public static PdfColor Ivory = new PdfColor(255, 255, 240, 255);
        public static PdfColor Khaki = new PdfColor(240, 230, 140, 255);
        public static PdfColor Lavender = new PdfColor(230, 230, 250, 255);
        public static PdfColor LavenderBlush = new PdfColor(255, 240, 245, 255);
        public static PdfColor LawnGreen = new PdfColor(124, 252, 0, 255);
        public static PdfColor LemonChiffon = new PdfColor(255, 250, 205, 255);
        public static PdfColor LightBlue = new PdfColor(173, 216, 230, 255);
        public static PdfColor LightCoral = new PdfColor(240, 128, 128, 255);
        public static PdfColor LightCyan = new PdfColor(224, 255, 255, 255);
        public static PdfColor LightGoldenrodYellow = new PdfColor(250, 250, 210, 255);
        public static PdfColor LightGreen = new PdfColor(144, 238, 144, 255);
        public static PdfColor LightGray = new PdfColor(211, 211, 211, 255);
        public static PdfColor LightPink = new PdfColor(255, 182, 193, 255);
        public static PdfColor LightSalmon = new PdfColor(255, 160, 122, 255);
        public static PdfColor LightSeaGreen = new PdfColor(32, 178, 170, 255);
        public static PdfColor LightSkyBlue = new PdfColor(135, 206, 250, 255);
        public static PdfColor LightSlateGray = new PdfColor(119, 136, 153, 255);
        public static PdfColor LightSteelBlue = new PdfColor(176, 196, 222, 255);
        public static PdfColor LightYellow = new PdfColor(255, 255, 224, 255);
        public static PdfColor Lime = new PdfColor(0, 255, 0, 255);
        public static PdfColor LimeGreen = new PdfColor(50, 205, 50, 255);
        public static PdfColor Linen = new PdfColor(250, 240, 230, 255);
        public static PdfColor Magenta = new PdfColor(255, 0, 255, 255);
        public static PdfColor Maroon = new PdfColor(128, 0, 0, 255);
        public static PdfColor MediumAquamarine = new PdfColor(102, 205, 170, 255);
        public static PdfColor MediumBlue = new PdfColor(0, 0, 205, 255);
        public static PdfColor MediumOrchid = new PdfColor(186, 85, 211, 255);
        public static PdfColor MediumPurple = new PdfColor(147, 112, 219, 255);
        public static PdfColor MediumSeaGreen = new PdfColor(60, 179, 113, 255);
        public static PdfColor MediumSlateBlue = new PdfColor(123, 104, 238, 255);
        public static PdfColor MediumSpringGreen = new PdfColor(0, 250, 154, 255);
        public static PdfColor MediumTurquoise = new PdfColor(72, 209, 204, 255);
        public static PdfColor MediumVioletRed = new PdfColor(199, 21, 133, 255);
        public static PdfColor MidnightBlue = new PdfColor(25, 25, 112, 255);
        public static PdfColor MintCream = new PdfColor(245, 255, 250, 255);
        public static PdfColor MistyRose = new PdfColor(255, 228, 225, 255);
        public static PdfColor Moccasin = new PdfColor(255, 228, 181, 255);
        public static PdfColor NavajoWhite = new PdfColor(255, 222, 173, 255);
        public static PdfColor Navy = new PdfColor(0, 0, 128, 255);
        public static PdfColor OldLace = new PdfColor(253, 245, 230, 255);
        public static PdfColor Olive = new PdfColor(128, 128, 0, 255);
        public static PdfColor OliveDrab = new PdfColor(107, 142, 35, 255);
        public static PdfColor Orange = new PdfColor(255, 165, 0, 255);
        public static PdfColor OrangeRed = new PdfColor(255, 69, 0, 255);
        public static PdfColor Orchid = new PdfColor(218, 112, 214, 255);
        public static PdfColor PaleGoldenrod = new PdfColor(238, 232, 170, 255);
        public static PdfColor PaleGreen = new PdfColor(152, 251, 152, 255);
        public static PdfColor PaleTurquoise = new PdfColor(175, 238, 238, 255);
        public static PdfColor PaleVioletRed = new PdfColor(219, 112, 147, 255);
        public static PdfColor PapayaWhip = new PdfColor(255, 239, 213, 255);
        public static PdfColor PeachPuff = new PdfColor(255, 218, 185, 255);
        public static PdfColor Peru = new PdfColor(205, 133, 63, 255);
        public static PdfColor Pink = new PdfColor(255, 192, 203, 255);
        public static PdfColor Plum = new PdfColor(221, 160, 221, 255);
        public static PdfColor PowderBlue = new PdfColor(176, 224, 230, 255);
        public static PdfColor Purple = new PdfColor(128, 0, 128, 255);
        public static PdfColor Red = new PdfColor(255, 0, 0, 255);
        public static PdfColor RosyBrown = new PdfColor(188, 143, 143, 255);
        public static PdfColor RoyalBlue = new PdfColor(65, 105, 225, 255);
        public static PdfColor SaddleBrown = new PdfColor(139, 69, 19, 255);
        public static PdfColor Salmon = new PdfColor(250, 128, 114, 255);
        public static PdfColor SandyBrown = new PdfColor(244, 164, 96, 255);
        public static PdfColor SeaGreen = new PdfColor(46, 139, 87, 255);
        public static PdfColor SeaShell = new PdfColor(255, 245, 238, 255);
        public static PdfColor Sienna = new PdfColor(160, 82, 45, 255);
        public static PdfColor Silver = new PdfColor(192, 192, 192, 255);
        public static PdfColor SkyBlue = new PdfColor(135, 206, 235, 255);
        public static PdfColor SlateBlue = new PdfColor(106, 90, 205, 255);
        public static PdfColor SlateGray = new PdfColor(112, 128, 144, 255);
        public static PdfColor Snow = new PdfColor(255, 250, 250, 255);
        public static PdfColor SpringGreen = new PdfColor(0, 255, 127, 255);
        public static PdfColor SteelBlue = new PdfColor(70, 130, 180, 255);
        public static PdfColor Tan = new PdfColor(210, 180, 140, 255);
        public static PdfColor Teal = new PdfColor(0, 128, 128, 255);
        public static PdfColor Thistle = new PdfColor(216, 191, 216, 255);
        public static PdfColor Tomato = new PdfColor(255, 99, 71, 255);
        public static PdfColor Turquoise = new PdfColor(64, 224, 208, 255);
        public static PdfColor Violet = new PdfColor(238, 130, 238, 255);
        public static PdfColor Wheat = new PdfColor(245, 222, 179, 255);
        public static PdfColor White = new PdfColor(255, 255, 255, 255);
        public static PdfColor WhiteSmoke = new PdfColor(245, 245, 245, 255);
        public static PdfColor Yellow = new PdfColor(255, 255, 0, 255);
        public static PdfColor YellowGreen = new PdfColor(154, 205, 50, 255);
    }
}
