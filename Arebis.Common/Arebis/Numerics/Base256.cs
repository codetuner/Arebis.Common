using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Numerics
{
    /// <summary>
    /// A numeral system of base 256.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Base256 : NumeralSystem
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        public static Base256 Instance = new Base256();

        /// <summary>
        /// Creates a numeral system of base 256.
        /// </summary>
        public Base256()
            : base(256)
        { }
        
        public static byte[] ToBytes(string b256)
        {
            if (b256 == null) return null;

            var arr = new byte[b256.Length];
            for(int i=0; i<b256.Length; i++)
            {
                arr[i] = (byte)b256[i];
            }

            return arr;
        }

        public static string FromBytes(byte[] bytes)
        {
            if (bytes == null) return null;
            if (bytes.Length == 0) return String.Empty;

            var arr = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                arr[i] = (char)bytes[i];
            }

            return new String(arr);
        }
    }
}
