using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arebis.Imaging
{
    public class ImageResizer
    {
        public ConcurrentQueue<FileResizeItem> ImageQueue { get; private set; }

        public int ThreadCount { get; set; }

        public bool AbortRequested { get; set; }

        public ImageResizer()
        {
            ImageQueue = new ConcurrentQueue<FileResizeItem>();
            ThreadCount = Math.Min(Environment.ProcessorCount + 2, 10);
        }

        public void Enqueue(FileResizeItem item)
        {
            this.ImageQueue.Enqueue(item);
        }

        public void Run()
        {
            var threads = new List<Thread>();
            while (threads.Count < (this.ThreadCount))
            {
                var thread = new Thread(this.ThreadStart);
                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            AbortRequested = false;
        }

        protected void ThreadStart()
        {
            FileResizeItem item;
            while (this.ImageQueue.TryDequeue(out item))
            {
                if (this.AbortRequested) break;
                ResizeFile(item);
            }
        }

        public static Bitmap ResizeImage(Bitmap image, int newWidth, int newHeight, int imageWidth, int imageHeight, int paddingLeft, int paddingTop)
        {
            //lock ("ImageResizer.CreateThumbnail.Lock")
            {
                System.Drawing.Bitmap bmpOut = null;
                //ImageFormat loFormat = image.RawFormat;
                bmpOut = new Bitmap(newWidth, newHeight);
                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
                //g.DrawImage(image, 0, 0, newWidth, newHeight);
                g.DrawImage(image, -paddingLeft - 1, -paddingTop - 1, imageWidth + 2, imageHeight + 2);

                return bmpOut;
            }
        }

        public static void ResizeFile(FileResizeItem item)
        {
            // Input file name:
            var inputfile = item.SourceFile.FullName;

            // Determine target file name:
            var targetFile = DetermineTargetFile(item.SourceFile, item.TargetFile, item.TargetWidth, item.TargetHeight);

            // Skip file if already exists:
            if (item.Overwrite == false && File.Exists(targetFile)) return;

            // Configure JPEG encoding:
            var jpegEncoder = GetCodecInfo(ImageFormat.Jpeg);
            var jpegEncoderParameters = new EncoderParameters(1);
            jpegEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Convert.ToInt64(item.Quality * 100));

            using (var image = new Bitmap(inputfile))
            {
                // Define new image sizes:
                var targetSize = new Size(item.TargetWidth, item.TargetHeight);
                var destinationImageSize = image.Size.Resize(targetSize, item.ResizeMode, item.OrientationMode);
                var destinationFileSize = destinationImageSize;
                var cropSize = new Size(0, 0);

                // Perform cropping:
                if (item.ResizeMode == ResizeMode.BoxFitsInCropped)
                {
                    destinationFileSize = targetSize;
                    cropSize = new Size((destinationImageSize.Width - destinationFileSize.Width) / 2, (destinationImageSize.Height - destinationFileSize.Height) / 2);
                }

                // Perform resizing:
                using (var result = ResizeImage(image, destinationFileSize.Width, destinationFileSize.Height, destinationImageSize.Width, destinationImageSize.Height, cropSize.Width, cropSize.Height))
                {
                    // Copy metadata if requested to keep:
                    if (item.KeepMetaData)
                    {
                        PropertyItem orientationProperty = null;
                        foreach (var prop in image.PropertyItems)
                        {
                            if (prop.Id == 0x112)
                            {
                                orientationProperty = prop;
                            }
                            result.SetPropertyItem(prop);
                        }

                        if (item.AutoRotate && orientationProperty != null && orientationProperty.Value[0] != 1)
                        {
                            result.RotateFlip(OrientationToFlipType(orientationProperty.Value[0]));
                            orientationProperty.Value[0] = 1;
                            result.SetPropertyItem(orientationProperty);
                        }
                    }

                    // Save image:
                    EnsurePathExists(new FileInfo(targetFile).Directory);
                    result.Save(targetFile, jpegEncoder, jpegEncoderParameters);
                }
            }
        }

        private static string DetermineTargetFile(FileInfo sourceFile, string destinationFile, int targetWidth, int targetHeight)
        {
            destinationFile = destinationFile
                .Replace("{filename}", sourceFile.Name.Substring(0, sourceFile.Name.Length - sourceFile.Extension.Length))
                .Replace("{extension}", sourceFile.Extension)
                .Replace("{requestedwidth}", targetWidth.ToString())
                .Replace("{requestedheight}", targetHeight.ToString());

            destinationFile = Path.Combine(sourceFile.Directory.FullName, destinationFile);

            return destinationFile;
        }

        private static void EnsurePathExists(DirectoryInfo dir)
        {
            if (dir.Exists) return;
            EnsurePathExists(dir.Parent);
            dir.Create();
        }

        private static ImageCodecInfo GetCodecInfo(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private static RotateFlipType OrientationToFlipType(int orientation)
        {
            // Source: http://automagical.rationalmind.net/2009/08/25/correct-photo-orientation-using-exif/
            switch (orientation)
            {
                case 1:
                    return RotateFlipType.RotateNoneFlipNone;
                case 2:
                    return RotateFlipType.RotateNoneFlipX;
                case 3:
                    return RotateFlipType.Rotate180FlipNone;
                case 4:
                    return RotateFlipType.Rotate180FlipX;
                case 5:
                    return RotateFlipType.Rotate90FlipX;
                case 6:
                    return RotateFlipType.Rotate90FlipNone;
                case 7:
                    return RotateFlipType.Rotate270FlipX;
                case 8:
                    return RotateFlipType.Rotate270FlipNone;
                default:
                    return RotateFlipType.RotateNoneFlipNone;
            }
        }
    }
}
