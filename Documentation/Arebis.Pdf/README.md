Arebis.Pdf
==========

.NET PDF generation library written in C#

Installation
------------

Install the "Arebis.Pdf" NuGet package in your .NET project.

See: https://www.nuget.org/packages/Arebis.Pdf/

Source code is available on: https://github.com/codetuner/Arebis.Common

Features and Limitations
------------------------

For a quick impression of the capabilities of this library, [see this sample PDF file created by the library](https://raw.githubusercontent.com/codetuner/Arebis.Common/master/Documentation/Arebis.Pdf/Sample1.pdf).

Features of the library include:

- Ability to write PDF streamed in single pass.
- Supports chuncked transfer on ASP.NET streams.
- Drawing of text with rich features.
- Drawing of lines, (rounded)rectangles, ovals, circles, with rich features.
- Drawing of images with image position control and rotation.
- Text wrapping.
- Support for template objects (including image objects) to be added
  once in the document, and rendered on multiple pages.
- Extendible.
- Open source.
- ...

But be aware that the library also has some important limitations:

- Only support for standard PDF fonts.
- Filling of drawed objects only in plain color.
- Text wrapping is supported, but not page wrapping.
- ...

I tried to create a properly designed component that is extendible. You should therefore be able to add
missing features without touching the code. But of course, you are welcome to contribute new features
to the code base!

Sample Usage
------------

A simple "Hello World" document can be created with the following code:

    // Create a TextOptions object:
    var to = new PdfTextOptions(PdfPredefinedFont.Helvetica, 12.0, PdfColor.Blue);
    
    // Write the document:
    using (var stream = new FileStream(@"HelloWorld.pdf", FileMode.Create, FileAccess.Write))
    using (var writer = new PdfDocumentWriter(stream))
    {
        // Write a page:
        using (var page = writer.NewPage(PdfPageFormat.A4Portrait))
        {
            // Draw text on the page (with given TextOptions):
            page.DrawText(40, 800, "Hello World !", to);
        }
    }

Why another PDF library
-----------------------

Why another PDF library you may think... Well, I needed a PDF library that was light-weight, allowed to
write a PDF streamed in single pass, could reference images on all pages without repeating the image BLOB
on every page, contain rotated text, boxes and dashed lines.

Although some libraries came close, I did not find a library that matched all my requirements. Next I tried
to extend an exiting open source library, but soon I found the design of that component to be so flawwed that
a major refactoring would be needed.

With the closer look to that library, I discovered that writing raw PDF is not that hard at the end, and so 
ended up creating yet another PDF library...


Release notes
-------------

1.4.0.0 : Added support for rotation of images and rectangles; added support for rounded rectangles.
