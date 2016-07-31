using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Extensions
{
    public static class NumberExtension
    {
        private const string hexalfabet = "0123456789ABCDEF";

        public static string ToHex(this byte value)
        {
            return String.Empty + hexalfabet[value / 16] + hexalfabet[value % 16];
        }

        public static string ToHex(this byte[] value)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                var result = new StringBuilder();
                for (int i = 0; i < value.Length; i++)
                {
                    result.Append(hexalfabet[value[i] / 16]);
                    result.Append(hexalfabet[value[i] % 16]);
                }
                return result.ToString();
            }
        }

        [CLSCompliant(false)]
        public static string ToHex(this System.UInt32 value, int minlength = 1)
        {
            var result = new StringBuilder();
            while (value != 0)
            {
                uint l = (value & 0xF);
                result.Insert(0, hexalfabet[(int)l]);

                value >>= 4;

                uint h = (value & 0xF);
                result.Insert(0, hexalfabet[(int)h]);

                value >>= 4;
            }

            while (result.Length < minlength)
                result.Insert(0, '0');

            return result.ToString();
        }

        /// <summary>
        /// Translates an integer into values based on their index. If no value for the index is
        /// found, default(T) is returned.
        /// </summary>
        public static T Translate<T>(this int value, params T[] indexValues)
        {
            if (value < 0) return default(T);
            if (indexValues == null) return default(T);
            if (value >= indexValues.Length) return default(T);
            return indexValues[value];
        }

        /// <summary>
        /// Returns the ceiling integer result of a division.
        /// </summary>
        /// <param name="value">The total value to be devided in parts.</param>
        /// <param name="partSize">The size of a part.</param>
        /// <returns>The number of parts needed to contain the total value.</returns>
        public static int CeilingDiv(this int value, int partSize)
        {
            return (value + partSize - 1) / partSize;
        }

        /// <summary>
        /// Whether the value is between the two bounderies (boundaries included).
        /// </summary>
        public static bool IsBetween(this double d, double lower, double higher)
        {
            return (d >= lower && d <= higher);
        }

        /// <summary>
        /// Whether the value is between the two bounderies (boundaries included).
        /// </summary>
        public static bool IsBetween(this int i, int lower, int higher)
        {
            return (i >= lower && i <= higher);
        }

    }
}
