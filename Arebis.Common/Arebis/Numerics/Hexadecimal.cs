using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Numerics
{
    /// <summary>
    /// A numeral system with 16 symbols (0-9 and A-F).
    /// </summary>
    [DataContract]
    [Serializable]
    public class Hexadecimal : SymbolicNumeralSystem
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        public static Hexadecimal Instance = new Hexadecimal();

        /// <summary>
        /// Creates a numeral system with 16 symbols (0-9 and A-F).
        /// </summary>
        public Hexadecimal()
            : base(16)
        { }

        public static byte[] ToBytes(string hex)
        {
            if (hex == null) return null;
            if (hex.Length % 2 == 1) hex = "0" + hex;

            Func<int, int> getHexVal = (int h) => { return h - (h < 58 ? 48 : (h < 97 ? 55 : 87)); };

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((getHexVal(hex[i << 1]) << 4) + (getHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static string FromBytes(byte[] values, string separator = "")
        {
            var symbols = "0123456789ABCDEF";

            if (values == null) return null;
            if (values.Length == 0) return String.Empty;
            separator = separator ?? String.Empty;

            StringBuilder sb = new StringBuilder();
            var actualSeparator = String.Empty;
            foreach(var value in values)
            {
                sb.Append(actualSeparator);
                sb.Append(symbols[value >> 4]);
                sb.Append(symbols[value % 16]);
                actualSeparator = separator;
            }

            return sb.ToString();
        }

        public override string PrepareForParse(string s)
        {
            return s.ToUpperInvariant();
        }
    }
}
