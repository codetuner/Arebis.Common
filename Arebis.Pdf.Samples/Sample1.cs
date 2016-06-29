using Arebis.Pdf.Common;
using Arebis.Pdf.Writing;
using System.IO;

namespace Arebis.Pdf.Samples
{
    public static class Sample1
    {
        public static void Run(string outputfilename)
        {
            // Prepare document options:
            var options = new PdfDocumentOptions();
            options.Author = "Arebis";
            options.Title = "PDF Generation Sample";
            options.Subject = "Demonstrate Arebis.Pdf generation library.";
            options.TextFilter = new Arebis.Pdf.Common.PdfDeflateStreamFilter();
            //options.TextFilter = new Arebis.Pdf.Common.PdfASCIIHexDecodeFilter();

            // Make stream and writer objects:
            using (var stream = new FileStream(outputfilename, FileMode.Create, FileAccess.Write))
            using (var writer = new PdfDocumentWriter(stream, options))
            {
                // Load an image to place on all pages:
                var logo = Properties.Resources.ArebisLogo160;
                var logoref = writer.AddImage(logo);
                
                // Create a page header object:
                var header = new PdfScriptObject();
                header.BeginGraphicsState();
                header.SetStrokeColor(PdfColor.Black);
                header.SetFillColor(PdfColor.Black);
                header.SetStrokeWidth(1);
                header.BeginText(50, 810, PdfPredefinedFont.TimesRomanItalic, 10.0);
                header.DrawText("Arebis.Pdf Library - Sample 1");
                header.EndText();
                header.DrawImageByRef(465, 25, 100, 100 * logo.Height / logo.Width, logoref);
                header.BeginPath(50, 808);
                header.DrawLineA(50, 808, 0, 500);
                header.EndPath(true, true, false);
                header.EndGraphicsState();
                var headerref = writer.WriteObject(header);

                // Add a page with text:
                using (var page = writer.NewPage(PdfPageFormat.A4Portrait))
                {
                    // Write header:
                    page.WriteObjectRef(headerref);

                    // Text options template:
                    var h1 = new PdfTextOptions(PdfPredefinedFont.HelveticaBold, 36);
                    var p = new PdfTextOptions(PdfPredefinedFont.TimesRoman, 12);

                    // Write title:
                    page.DrawText(50, 760, "Lorem Ipsum", h1);

                    // Draw box (box has no function, just to show) and write text splitted to fit:
                    page.DrawRectangle(45, 255, 510, 480, new PdfGraphicsOptions(0, PdfColor.Gray));
                    page.DrawTextblock(50, 720, Properties.Resources.LoremIpsum, 500, p);

                }
                
                // Add a page with font effects:
                using (var page = writer.NewPage(PdfPageFormat.A4Portrait))
                {
                    var text1 = "The quick brown fox jumps over the lazy dog";
                    var text2 = "Åxèl Dößeçais won € 25.95";

                    // Write header:
                    page.WriteObjectRef(headerref);

                    // Text options template:
                    var to1 = new PdfTextOptions(PdfPredefinedFont.HelveticaBold, 23.3, PdfColor.Blue, PdfTextRenderingMode.Fill, PdfColor.LightBlue, 1.6);
                    var to2 = new PdfTextOptions(PdfPredefinedFont.HelveticaBold, 14.0);
                    var to3 = new PdfTextOptions(PdfPredefinedFont.HelveticaItalic, 8, PdfColor.Gray);

                    // Draw text with different effects:
                    page.DrawText(50, 760, text1, to1);
                    page.DrawText(50, 720, text1, new PdfTextOptions(to1) { InkColor = PdfColor.Red });
                    page.DrawText(50, 680, text1, new PdfTextOptions(to1) { RenderingMode = PdfTextRenderingMode.FillAndStroke });
                    page.DrawText(50, 640, text1, new PdfTextOptions(to1) { RenderingMode = PdfTextRenderingMode.Stroke });
                    page.DrawText(50, 600, text1, new PdfTextOptions(to1) { RenderingMode = PdfTextRenderingMode.Stroke, LineDashPattern = PdfLineDashPattern.Medium });
                    page.DrawText(50, 560, text1, new PdfTextOptions(to1) { RenderingMode = PdfTextRenderingMode.Stroke, OutlineWidth = 0.1, OutlineColor = PdfColor.Red });

                    // Draw with different fonts:
                    page.DrawText(50, 600 - 80, text1, new PdfTextOptions(to2) { Font = PdfPredefinedFont.Courier });
                    page.DrawText(50, 585 - 80, text1, new PdfTextOptions(to2) { Font = PdfPredefinedFont.CourierItalic });
                    page.DrawText(50, 570 - 80, text1, new PdfTextOptions(to2) { Font = PdfPredefinedFont.CourierBold });
                    page.DrawText(50, 555 - 80, text1, new PdfTextOptions(to2) { Font = PdfPredefinedFont.CourierBoldItalic });
                    page.DrawText(50, 525 - 80, text1, new PdfTextOptions(to2) { Font = PdfPredefinedFont.Helvetica });
                    page.DrawText(50, 510 - 80, text1, new PdfTextOptions(to2) { Font = PdfPredefinedFont.HelveticaItalic });
                    page.DrawText(50, 495 - 80, text1, new PdfTextOptions(to2) { Font = PdfPredefinedFont.HelveticaBold });
                    page.DrawText(50, 480 - 80, text1, new PdfTextOptions(to2) { Font = PdfPredefinedFont.HelveticaBoldItalic });
                    page.DrawText(50, 450 - 80, text1, new PdfTextOptions(to2) { Font = PdfPredefinedFont.TimesRoman });
                    page.DrawText(50, 435 - 80, text1, new PdfTextOptions(to2) { Font = PdfPredefinedFont.TimesRomanItalic });
                    page.DrawText(50, 420 - 80, text1, new PdfTextOptions(to2) { Font = PdfPredefinedFont.TimesRomanBold });
                    page.DrawText(50, 405 - 80, text1, new PdfTextOptions(to2) { Font = PdfPredefinedFont.TimesRomanBoldItalic });

                    // Draw rotated text:
                    page.DrawText(65, 50, text2, new PdfTextOptions(to2) { InkColor = PdfColor.DarkOliveGreen });
                    page.DrawText(65, 70, text2, new PdfTextOptions(to2) { InkColor = new PdfColor(200, 200, 200) });
                    page.DrawText(65, 70, text2, new PdfTextOptions(to2) { InkColor = new PdfColor(150, 150, 150) }, 22.0);
                    page.DrawText(65, 70, text2, new PdfTextOptions(to2) { InkColor = new PdfColor(100,100,100) }, 45.0);
                    page.DrawText(65, 70, text2, new PdfTextOptions(to2) { InkColor = new PdfColor(50, 50, 50) }, 67.0);
                    page.DrawText(65, 70, text2, new PdfTextOptions(to2) { InkColor = new PdfColor(0, 0, 0) }, 90.0);

                    // Draw big 'A' with different LineCapStyles:
                    page.DrawText(240, 170, "A", new PdfTextOptions(to1) { FontSize = 144.0, RenderingMode = PdfTextRenderingMode.Stroke, OutlineWidth = 8, OutlineColor = PdfColor.Red, LineDashPattern = PdfLineDashPattern.XLarge, LineCapStyle = PdfLineCapStyle.Butt });
                    page.DrawText(340, 170, "A", new PdfTextOptions(to1) { FontSize = 144.0, RenderingMode = PdfTextRenderingMode.Stroke, OutlineWidth = 8, OutlineColor = PdfColor.Green, LineDashPattern = PdfLineDashPattern.XLarge, LineCapStyle = PdfLineCapStyle.Round });
                    page.DrawText(440, 170, "A", new PdfTextOptions(to1) { FontSize = 144.0, RenderingMode = PdfTextRenderingMode.Stroke, OutlineWidth = 8, OutlineColor = PdfColor.Blue, LineDashPattern = PdfLineDashPattern.XLarge, LineCapStyle = PdfLineCapStyle.Square });
                    // Legend:
                    page.DrawText(240, 155, "LineCapStyle.Butt", new PdfTextOptions(to3) { InkColor = PdfColor.Red });
                    page.DrawText(340, 155, "LineCapStyle.Round", new PdfTextOptions(to3) { InkColor = PdfColor.Green });
                    page.DrawText(440, 155, "LineCapStyle.Square", new PdfTextOptions(to3) { InkColor = PdfColor.Blue });
                }

                // Add a page with drawings:
                using (var page = writer.NewPage(PdfPageFormat.A4Portrait))
                {
                    // Write header:
                    page.WriteObjectRef(headerref);

                    // Graphical options template:
                    var helpline = new PdfGraphicsOptions(0.0, PdfColor.Gray, PdfColor.White, PdfLineDashPattern.Small);
                    var got1 = new PdfGraphicsOptions(2.0, PdfColor.Black, PdfColor.White, PdfLineDashPattern.Solid);

                    // Draw rectangles:
                    var got2 = new PdfGraphicsOptions(got1) { LineDashPattern = PdfLineDashPattern.Medium, FillColor = PdfColor.Yellow };
                    page.DrawRectangle(50, 700, 240, 80, got1);
                    page.DrawRoundedRectangle(50 + 10, 700 + 10, 240 - 20, 80 - 20, 15, got2);
                    page.DrawRectangle(50 + 240 + 10 + 10, 700, 240, 80, got2);
                    page.DrawRoundedRectangle(50 + 240 + 10 + 10 + 10, 700 + 10, 240 - 20, 80 - 20, 15, got1);


                    // Draw oval in rectangle:
                    page.DrawRectangle(50, 600, 400, 80, helpline);
                    page.DrawOval(50, 600, 400, 80, got1);

                    // Draw circle in rectangle:
                    page.DrawRectangle(550 - 80, 600, 80, 80, helpline);
                    page.DrawCircle(550 - 40, 600 + 40, 40, got1);

                    // Draw various line types:
                    page.DrawLine(50, 560, 550, 560, new PdfGraphicsOptions(8.0, PdfColor.Goldenrod));
                    page.DrawLine(50, 540, 550, 540, new PdfGraphicsOptions(4.0, PdfColor.Green));
                    page.DrawLine(50, 520, 550, 520, new PdfGraphicsOptions(2.0, PdfColor.Brown));
                    page.DrawLine(50, 500, 550, 500, new PdfGraphicsOptions(1.0, PdfColor.Coral));
                    page.DrawLine(50, 480, 550, 480, new PdfGraphicsOptions(0.5, PdfColor.Lime));
                    page.DrawLine(50, 460, 550, 460, new PdfGraphicsOptions(0.0, PdfColor.Magenta));

                    // Dashed lines (with the last one, a custom dash definition):
                    page.DrawLine(50, 420, 550, 420, new PdfGraphicsOptions(1.0, PdfColor.Red, null, PdfLineDashPattern.Large));
                    page.DrawLine(50, 400, 550, 400, new PdfGraphicsOptions(1.0, PdfColor.Green, null, PdfLineDashPattern.Medium));
                    page.DrawLine(50, 380, 550, 380, new PdfGraphicsOptions(1.0, PdfColor.Blue, null, PdfLineDashPattern.Small));
                    page.DrawLine(50, 360, 550, 360, new PdfGraphicsOptions(1.0, PdfColor.Gray, null, new PdfLineDashPattern(2, 8)));

                    // Circle of lines:
                    var cl = new PdfScriptObject();
                    cl.BeginGraphicsState();
                    cl.SetStrokeColor(PdfColor.Gold);
                    cl.SetStrokeWidth(2.0);
                    for (int a = 0; a < 360; a += 15)
                    {
                        cl.DrawLineA(150, 220, a, 100);
                    }
                    cl.EndPath(false, true, false);
                    cl.EndGraphicsState();
                    page.WriteObject(cl);

                    // Circle of squares:
                    var got3 = new PdfGraphicsOptions(got1) { StrokeColor = PdfColor.Green };
                    for (int a = 15; a <= 360; a += 15)
                    {
                        page.DrawRectangle(450, 220, 70, 70, got3, a);
                    }
                }

                // Add a page with images:
                var imgHRef = writer.AddImage(Properties.Resources.ImgH);
                var imgVRef = writer.AddImage(Properties.Resources.ImgV);
                using (var page = writer.NewPage(PdfPageFormat.A4Portrait))
                {
                    // Write header:
                    page.WriteObjectRef(headerref);

                    // Graphical options template:
                    var bo = new PdfGraphicsOptions(0.5, PdfColor.Gray, null, PdfLineDashPattern.Medium);
                    var to = new PdfTextOptions(PdfPredefinedFont.HelveticaItalic, 8, PdfColor.Gray);

                    // First row: a vertical image:
                    page.DrawRectangle(48, 658, 114, 114, bo);
                    page.DrawImageRef(50, 660, imgVRef, 110, 110, PdfImagePlacement.Stretch);
                    page.DrawText(48, 650, "Stretch", to);

                    page.DrawRectangle(48 + 130, 658, 114, 114, bo);
                    page.DrawImageRef(50 + 130, 660, imgVRef, 110, 110, PdfImagePlacement.Center);
                    page.DrawText(48+130, 650, "Center", to);

                    page.DrawRectangle(48 + 260, 658, 114, 114, bo);
                    page.DrawImageRef(50 + 260, 660, imgVRef, 110, 110, PdfImagePlacement.LeftOrTop);
                    page.DrawText(48+260, 650, "LeftOrTop", to);

                    page.DrawRectangle(48 + 390, 658, 114, 114, bo);
                    page.DrawImageRef(50 + 390, 660, imgVRef, 110, 110, PdfImagePlacement.RightOrBottom);
                    page.DrawText(48+390, 650, "RightOrBottom", to);

                    // Second row: a horizontal image:
                    page.DrawRectangle(48, 508, 114, 114, bo);
                    page.DrawImageRef(50, 510, imgHRef, 110, 110, PdfImagePlacement.Stretch);
                    page.DrawText(48, 500, "Stretch", to);

                    page.DrawRectangle(48 + 130, 508, 114, 114, bo);
                    page.DrawImageRef(50 + 130, 510, imgHRef, 110, 110, PdfImagePlacement.Center);
                    page.DrawText(48 + 130, 500, "Center", to);

                    page.DrawRectangle(48 + 260, 508, 114, 114, bo);
                    page.DrawImageRef(50 + 260, 510, imgHRef, 110, 110, PdfImagePlacement.LeftOrTop);
                    page.DrawText(48 + 260, 500, "LeftOrTop", to);

                    page.DrawRectangle(48 + 390, 508, 114, 114, bo);
                    page.DrawImageRef(50 + 390, 510, imgHRef, 110, 110, PdfImagePlacement.RightOrBottom);
                    page.DrawText(48 + 390, 500, "RightOrBottom", to);

                    // Third row: a rotated image:
                    page.DrawRectangle(48, 358, 114, 114, bo);
                    page.DrawImageRef(50, 360, imgHRef, 110, 110, PdfImagePlacement.Center, PdfImageRotation.None);
                    page.DrawText(48, 350, "No rotation", to);

                    page.DrawRectangle(48 + 130, 358, 114, 114, bo);
                    page.DrawImageRef(50 + 130, 360, imgHRef, 110, 110, PdfImagePlacement.Center, PdfImageRotation.Left);
                    page.DrawText(48 + 130, 350, "Rotated Left", to);

                    page.DrawRectangle(48 + 260, 358, 114, 114, bo);
                    page.DrawImageRef(50 + 260, 360, imgHRef, 110, 110, PdfImagePlacement.Center, PdfImageRotation.Right);
                    page.DrawText(48 + 260, 350, "Rotated Right", to);

                    page.DrawRectangle(48 + 390, 358, 114, 114, bo);
                    page.DrawImageRef(50 + 390, 360, imgHRef, 110, 110, PdfImagePlacement.Center, PdfImageRotation.UpsideDown);
                    page.DrawText(48 + 390, 350, "Upside Down", to);

                    // Free rotation:
                    page.DrawImageRef(194.0, 90.0, imgVRef, 110.0);
                    page.DrawImageRef(194.0, 90.0, imgVRef, 110.0, 15.0);
                    page.DrawImageRef(194.0, 90.0, imgVRef, 110.0, 30.0);
                    page.DrawImageRef(194.0, 90.0, imgVRef, 110.0, 45.0);
                    page.DrawImageRef(194.0, 90.0, imgVRef, 110.0, 60.0);
                    page.DrawImageRef(194.0, 90.0, imgVRef, 110.0, 75.0);
                    page.DrawImageRef(194.0, 90.0, imgVRef, 110.0, 90.0);
                    page.DrawText(48, 80, "Free rotation", to);
                }
            }
        }
    }
}
