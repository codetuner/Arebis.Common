using Arebis.Pdf;
using Arebis.Pdf.Model;
using Arebis.Pdf.Writing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;

namespace Arebis.Pdf.Samples
{
    public static class Sample3
    {
        public static void Run(string outputfilename)
        {
            // Get the model, either by creating one or by deserializing one:
            var document = GetDocumentModel();

            // You can search and update the model in memory before generating the PDF. I.e:
            // Find the rectangle with id=titlebox and set/change it's radius:
            foreach (Rectangle item in document.All().Where(item => item.Id == "titlebox"))
            {
                item.Radius = "10";
            }

            // If you have localized data, you can provide an IDictionary<string,string> implementation
            // to resolve ContentKeys in the document model. Here we create the dictionary:
            var localizedSource = new Dictionary<string, string>();
            localizedSource["bodytext"] = "*************".Replace("*", "The quick brown fox jumps over the lazy dog. ");

            // You can use it for localized data, but also for any dynamic data. I.e:
            localizedSource["datetime"] = DateTime.Now.ToString();

            // Generate the PDF document:
            var pagenum = 0;
            using (var stream = new FileStream(outputfilename, FileMode.Create, FileAccess.Write))
            {
                // Create a new PdfModelWriter:
                var writer = new PdfModelWriter();

                // Set a ContentSource dictionary to resolve contentKeys which allow
                // for localization of the content of your documents:
                writer.ContentSource = localizedSource;

                // We can still intervene on document and page creation with callback actions. I.e:
                // Set a Document callback to dynamically inject an image when creating the document:
                writer.OnDocumentBegin = (docwriter, doc) =>
                {
                    var imgref = docwriter.AddImage(Properties.Resources.ImgV);
                    writer.SetReferenceObject("dynimg1", imgref);
                };

                // Or add page numbers:
                writer.OnPageEnd = (pagewriter, page) =>
                {
                    // Write "1" in the right bottom corner of the page, right-aligned:
                    pagewriter.DrawTextblock(pagewriter.Width - 120, 40, (++pagenum).ToString(), 60, (PdfTextOptions)writer.GetReferenceObject("t0"), 0.0, +1);
                };

                // Finally, write the document to the stream:
                writer.Write(document, stream);
            }
        }

        public static Document GetDocumentModel()
        {
            return new Document()
            {
                // Metadata of the document:
                Title = "Arebis.Pdf.Model Sample",
                Author = "Rudi Breedenraedt",
                // By default, use the "t0" Text Options for elements in this document:
                TextOptionsRef = "t0",
                // By default, use the "g0" Graphics Options for elements in this document:
                GraphicsOptionsRef = "g0",
                /// Items of the document:
                Items = {
                    // The default text options:
                    new TextOptions() {
                        Id = "t0",
                        FontRef = "Helvetica",
                        FontSize = "12"
                    },
                    // Text options based on t0 but with size 20:
                    new TextOptions() {
                        Id = "t1",
                        TemplateRef = "t0",
                        FontSize = "28"
                    },
                    // The default graphics options:
                    new GraphicsOptions() {
                        Id = "g0",
                        StrokeWidth = "1"
                    },
                    // A filled graphics options:
                    new GraphicsOptions() {
                        Id = "gf",
                        FillColor = "Black",
                    },
                    new Page() {
                        // Make an A4 sized page in portrait:
                        Format = "A4Portrait",
                        // A4 is 21cm width, use a coordinate space in this unit:
                        CoordinateSpace = new CoordinateSpace() { Width = 21.0 },
                        // Items of the page:
                        Items = {
                            // Using an Area to build a margin:
                            new Area() { X = "2", Y = "2", Width = "17", Height = "23.7",
                                // Items in the area:
                                Items = {
                                    // Write a rectangle:
                                    new Rectangle() { Id="titlebox", Height = "2.5", Width = "100%" },
                                    // Write text:
                                    new TextBlock() { Y = "0.6", Height = "2", Width = "100%", TextOptionsRef = "t1", Content = "Sample 3 : Based on a coded model", Alignment = "center" },
                                    // Adding a few 'checkboxes':
                                    new Area() { X = "0", Y = "3.2", Width = "0.4", Height = "0.4", Items = { new Rectangle(), new Cross() } }, // Checked box
                                    new Area() { X = "1", Y = "3.2", Width = "0.4", Height = "0.4", Items = { new Rectangle() } }, // Unchecked box
                                    new Area() { X = "2", Y = "3.2", Width = "0.4", Height = "0.4", Items = { new Oval() } }, // Circle
                                    new Area() { X = "3", Y = "3.2", Width = "0.4", Height = "0.4", Items = { new Oval() { GraphicsOptionsRef = "gf" } } }, // Filled Circle
                                    // Write text comming from the 'ContentSource' dictionary:
                                    new TextBlock() { Y = "4", Width = "48%", Alignment = "LEFT", ContentKey = "bodytext" },
                                    // Write an image, this image will be dynamically filled in by code:
                                    new Image() { X = "52%", Y = "4", Width = "48%", Height = "19.7", ImageRef = "dynimg1", Placement = "LeftOrTop" },
                                    // Place dynamic text on the bottom:
                                    new Text() { Y = "23.7", ContentKey = "datetime" }
                                }
                            }
                        }
                    }
                }
            };
        }

        public static void SaveModelWithDataContractSerializer(object model)
        {
            // Write the model as an XML file using DataContractSerializer:
            using (var stream = new FileStream(@"..\..\test-dcser.xml", FileMode.Create, FileAccess.Write))
            {
                new DataContractSerializer(model.GetType()).WriteObject(stream, model);
            }
        }

        public static void SaveModelWithXamlServices(object model)
        {
            // Write the model as an XML file using XamlServices:
            using (var stream = new FileStream(@"..\..\test-xaml.xml", FileMode.Create, FileAccess.Write))
            {
                XamlServices.Save(stream, model);
            }
        }
    }
}
