using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using Microsoft.Win32.SafeHandles;

namespace Arebis.WinApi
{
    public static class Kernel32
    {
        /// <summary>
        /// Retrieves file information for the specified file.
        /// </summary>
        /// <param name="hFile">File handle.</param>
        /// <param name="lpFileInformation">Returned file information.</param>
        /// <returns>Non-zero on success, zero on failure (then use 'Marshal.GetLastWin32Error').</returns>
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa364952(v=vs.85).aspx"/>
        [CLSCompliant(false)]
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

        /// <summary>
        /// Establishes a hard link between an existing file and a new file. This function is only supported on the NTFS file system, and only for files, not directories.
        /// </summary>
        /// <param name="lpFileName">The name of the new file.</param>
        /// <param name="lpExistingFileName">The name of the existing file.</param>
        /// <param name="lpSecurityAttributes">Reserved, must be IntPtr.Zero.</param>
        /// <returns>Non-zero on success, zero on failure (then use 'Marshal.GetLastWin32Error').</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        /// <summary>
        /// Gets volume information for the given path.
        /// </summary>
        /// <param name="RootPathName">A path in the volume to get info for. If path name is drive, it must end on an backslash.</param>
        /// <param name="VolumeNameBuffer"></param>
        /// <param name="VolumeNameSize"></param>
        /// <param name="VolumeSerialNumber"></param>
        /// <param name="MaximumComponentLength"></param>
        /// <param name="FileSystemFlags"></param>
        /// <param name="FileSystemNameBuffer"></param>
        /// <param name="nFileSystemNameSize"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetVolumeInformation(
          string RootPathName,
          StringBuilder VolumeNameBuffer,
          int VolumeNameSize,
          out uint VolumeSerialNumber,
          out uint MaximumComponentLength,
          out FileSystemFeature FileSystemFlags,
          StringBuilder FileSystemNameBuffer,
          int nFileSystemNameSize);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetVolumePathName(string lpszFileName, [Out] StringBuilder lpszVolumePathName, int cchBufferLength);

        [CLSCompliant(false)]
        public static void GetVolumeInformationOfPath(string itemPath, out string rootPathName, out string volumeName, out uint volumeSerialNumber, out FileSystemFeature features, out string fileSystemName, out uint maxComponentLength)
        {
            // Determine root path name:
            var rootPathNameBuilder = new StringBuilder(2000);
            Kernel32.GetVolumePathName(itemPath, rootPathNameBuilder, rootPathNameBuilder.Capacity);
            rootPathName = rootPathNameBuilder.ToString();

            // Get volumen information:
            StringBuilder volumeNameBuilder = new StringBuilder(261);
            StringBuilder fileSystemNameBuilder = new StringBuilder(261);
            if (!Kernel32.GetVolumeInformation(rootPathNameBuilder.ToString(), volumeNameBuilder, volumeNameBuilder.Capacity, out volumeSerialNumber, out maxComponentLength, out features, fileSystemNameBuilder, fileSystemNameBuilder.Capacity))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            volumeName = volumeNameBuilder.ToString();
            fileSystemName = fileSystemNameBuilder.ToString();
        }

        /// <summary>
        /// Enables an application to inform the system that it is in use, thereby preventing
        /// the system from entering sleep or turning off the display while the application is
        /// running.
        /// </summary>
        /// <param name="esFlags">The thread's execution requirements.</param>
        /// <returns>
        /// If the function succeeds, the return value is the previous thread execution state,
        /// if the function failes, NULL is returned.
        /// </returns>
        /// <see href="http://msdn.microsoft.com/en-us/library/aa373208(v=vs.85).aspx"/>
        [CLSCompliant(false)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

    }

    /// <summary>
    /// Contains information that the GetFileInformationByHandle function retrieves.
    /// </summary>
    [CLSCompliant(false)]
    [StructLayout(LayoutKind.Sequential)]
    public struct BY_HANDLE_FILE_INFORMATION
    {
        public uint FileAttributes;
        public ComTypes.FILETIME CreationTime;
        public ComTypes.FILETIME LastAccessTime;
        public ComTypes.FILETIME LastWriteTime;
        public uint VolumeSerialNumber;
        public uint FileSizeHigh;
        public uint FileSizeLow;
        public uint NumberOfLinks;
        public uint FileIndexHigh;
        public uint FileIndexLow;
    }

    [CLSCompliant(false)]
    [Flags]
    public enum FileSystemFeature : uint
    {
        /// <summary>
        /// The file system supports case-sensitive file names.
        /// </summary>
        CaseSensitiveSearch = 1,
        /// <summary>
        /// The file system preserves the case of file names when it places a name on disk.
        /// </summary>
        CasePreservedNames = 2,
        /// <summary>
        /// The file system supports Unicode in file names as they appear on disk.
        /// </summary>
        UnicodeOnDisk = 4,
        /// <summary>
        /// The file system preserves and enforces access control lists (ACL).
        /// </summary>
        PersistentACLS = 8,
        /// <summary>
        /// The file system supports file-based compression.
        /// </summary>
        FileCompression = 0x10,
        /// <summary>
        /// The file system supports disk quotas.
        /// </summary>
        VolumeQuotas = 0x20,
        /// <summary>
        /// The file system supports sparse files.
        /// </summary>
        SupportsSparseFiles = 0x40,
        /// <summary>
        /// The file system supports re-parse points.
        /// </summary>
        SupportsReparsePoints = 0x80,
        /// <summary>
        /// The specified volume is a compressed volume, for example, a DoubleSpace volume.
        /// </summary>
        VolumeIsCompressed = 0x8000,
        /// <summary>
        /// The file system supports object identifiers.
        /// </summary>
        SupportsObjectIDs = 0x10000,
        /// <summary>
        /// The file system supports the Encrypted File System (EFS).
        /// </summary>
        SupportsEncryption = 0x20000,
        /// <summary>
        /// The file system supports named streams.
        /// </summary>
        NamedStreams = 0x40000,
        /// <summary>
        /// The specified volume is read-only.
        /// </summary>
        ReadOnlyVolume = 0x80000,
        /// <summary>
        /// The volume supports a single sequential write.
        /// </summary>
        SequentialWriteOnce = 0x100000,
        /// <summary>
        /// The volume supports transactions.
        /// </summary>
        SupportsTransactions = 0x200000,
    }

    [CLSCompliant(false)]
    [FlagsAttribute]
    public enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
    }
}
