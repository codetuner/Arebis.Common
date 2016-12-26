using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Arebis.Extensions
{
    public static class ByteArrayExtension
    {
        /// <summary>
        /// Compares this byte array to the given one. Returns true if they are
        /// content-wise identical, false otherwise.
        /// </summary>
        public static bool CompareTo(this byte[] a, byte[] b)
        {
            if (Object.ReferenceEquals(a, b))
                return true;
            else if (Object.ReferenceEquals(a, null))
                return false;
            else if (Object.ReferenceEquals(b, null))
                return false;
            else if (a.Length != b.Length)
                return false;
            else
            {
                for (int i = 0; i < a.Length; i++)
                    if (a[i] != b[i])
                        return false;
                return true;
            }
        }
        
        /// <summary>
        /// Returns the given byte array compressed with GZip.
        /// </summary>
        public static byte[] GZipCompress(this byte[] source, CompressionLevel compressionLevel)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, compressionLevel, true))
                {
                    gzip.Write(source, 0, source.Length);
                }
                return memory.ToArray();
            }
        }
    }
}
