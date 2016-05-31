using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Arebis.IO
{
    public interface IFileSystemVisitor
    {
        bool Visit(DirectoryInfo folder);

        void Visited(DirectoryInfo folder);

        void Visit(FileInfo file);
    }
}
