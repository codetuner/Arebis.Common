using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Arebis.Imaging
{
    public class FileResizeItem
    {
        public FileInfo SourceFile { get; set; }

        public string TargetFile { get; set; }

        public int TargetWidth { get; set; }

        public int TargetHeight { get; set; }

        public OrientationMode OrientationMode { get; set; }

        public ResizeMode ResizeMode { get; set; }

        public bool KeepMetaData { get; set; }

        public bool AutoRotate { get; set; }

        public bool Overwrite { get; set; }

        public float Quality { get; set; }

        public FileResizeItem Clone()
        {
            return (FileResizeItem)this.MemberwiseClone();
        }
    }
}
