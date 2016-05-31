using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Arebis.WinApi;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace Arebis.Extensions
{
    public static class FileInfoExtension
    {
        public static string GetIdentifier(this FileInfo subject)
        {
            var fileinfo = FileInfoExtension.GetExtendedInformation(subject);
            return fileinfo.VolumeSerialNumber.ToHex(8) + ":" + fileinfo.FileIndexHigh.ToHex(8) + ":" + fileinfo.FileIndexLow.ToHex(8);
        }

        [CLSCompliant(false)]
        public static BY_HANDLE_FILE_INFORMATION GetExtendedInformation(this FileInfo subject)
        {
            using (var fs = File.Open(subject.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                BY_HANDLE_FILE_INFORMATION fileinfo;
                if (Kernel32.GetFileInformationByHandle(fs.SafeFileHandle, out fileinfo))
                {
                    return fileinfo;
                }
                else
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                    throw new ExternalException();
                }
            }
        }

        [CLSCompliant(false)]
        public static uint GetVolumeSerialNumber(this DirectoryInfo subject)
        {
            string volumeName, fileSystemName, rootpathname;
            FileSystemFeature features;
            uint volumeSerialNumber, maxComponentLength;
            Kernel32.GetVolumeInformationOfPath(subject.FullName, out rootpathname, out volumeName, out volumeSerialNumber, out features, out fileSystemName, out maxComponentLength);
            return volumeSerialNumber;
        }

        public static string GetVolumeName(this DirectoryInfo subject)
        {
            string volumeName, fileSystemName, rootpathname;
            FileSystemFeature features;
            uint volumeSerialNumber, maxComponentLength;
            Kernel32.GetVolumeInformationOfPath(subject.FullName, out rootpathname, out volumeName, out volumeSerialNumber, out features, out fileSystemName, out maxComponentLength);
            return volumeName;
        }

        /// <summary>
        /// Serializes the given object to the file using a BinaryFormatter.
        /// </summary>
        public static void WriteObject(this FileInfo file, object obj)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, obj);
            }
        }

        /// <summary>
        /// Deserializes an object from the given file using a BinaryFormatter.
        /// </summary>
        public static object ReadObject(this FileInfo file)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                return formatter.Deserialize(stream);
            }
        }
    }
}
