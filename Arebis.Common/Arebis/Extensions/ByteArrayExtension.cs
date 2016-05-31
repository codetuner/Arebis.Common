using System;
using System.Collections.Generic;
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
    }
}
