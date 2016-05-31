using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Arebis.IO
{
    public static class FileSystem
    {
        public static void Visit(IFileSystemVisitor visitor, string path, bool recurse)
        {
            if (Directory.Exists(path))
                Visit(visitor, new DirectoryInfo(path), recurse);
            else if (File.Exists(path))
                Visit(visitor, new FileInfo(path));
        }

        public static void Visit(IFileSystemVisitor visitor, DirectoryInfo folder, bool recurse)
        {
            if (Visit(visitor, folder))
            {
                FileInfo[] fileitems;
                DirectoryInfo[] diritems;
                //try
                //{
                    fileitems = folder.EnumerateFiles().ToArray();
                    diritems = (recurse) ? folder.EnumerateDirectories().ToArray() : null;
                //}
                //catch (Exception ex)
                //{
                //    visitor.OnException(folder, ex);
                //    fileitems = new FileInfo[0];
                //    diritems = new DirectoryInfo[0];
                //}

                foreach (var file in fileitems)
                {
                    Visit(visitor, file);
                }

                Visited(visitor, folder);

                if (recurse)
                {
                    foreach (var subfolder in diritems)
                    {
                        Visit(visitor, subfolder, recurse);
                    }
                }
            }
        }

        public static bool Visit(IFileSystemVisitor visitor, DirectoryInfo folder)
        {
            return visitor.Visit(folder);
        }

        public static void Visited(IFileSystemVisitor visitor, DirectoryInfo folder)
        {
            visitor.Visited(folder);
        }

        public static void Visit(IFileSystemVisitor visitor, FileInfo file)
        {
            visitor.Visit(file);
        }
    }
}
