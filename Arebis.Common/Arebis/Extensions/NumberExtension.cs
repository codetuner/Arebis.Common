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

        /// <summary>
        /// Splits a given value into equal parts rounded to a given number of decimals.
        /// The last part contains the remaining value ensuring that all parts add up to the original value without rounding errors.
        /// I.e. 2.0 rounded in 3 parts with 2 decimals will result in 0.67, 0.67 and 0.66.
        /// </summary>
        /// <param name="value">Value to split.</param>
        /// <param name="intoParts">Number of parts to split into (must be 1 or more).</param>
        /// <param name="roundingDecimals">Number of decimals to round the parts to.</param>
        /// <param name="roundingMode">Rounding mode to use. AwayFromZero by default.</param>
        /// <returns>Parts of the value.</returns>
        public static decimal[] Split(this decimal value, int intoParts, int roundingDecimals, MidpointRounding roundingMode = MidpointRounding.AwayFromZero)
        {
            if (intoParts <= 0) throw new ArgumentOutOfRangeException("intoParts", "The intoParts argument must not be less than 1.");

            var result = new decimal[intoParts];
            var part = Math.Round(value / intoParts, roundingDecimals, roundingMode);
            var remainder = value;

            for (int i = 0; i < (intoParts - 1); i++)
            {
                result[i] = part;
                remainder -= part;
            }

            result[intoParts - 1] = remainder;

            return result;
        }

        /// <summary>
        /// Splits a given value into equal parts rounded to a given number of decimals.
        /// The last part contains the remaining value ensuring that all parts add up to the original value without rounding errors.
        /// I.e. 2.0 rounded in 3 parts with 2 decimals will result in 0.67, 0.67 and 0.66.
        /// </summary>
        /// <param name="value">Value to split.</param>
        /// <param name="intoParts">Number of parts to split into (must be 1 or more).</param>
        /// <param name="roundingDecimals">Number of decimals to round the parts to.</param>
        /// <param name="roundingMode">Rounding mode to use. AwayFromZero by default.</param>
        /// <returns>Parts of the value.</returns>
        public static decimal?[] Split(this decimal? value, int intoParts, int roundingDecimals, MidpointRounding roundingMode = MidpointRounding.AwayFromZero)
        {
            if (value.HasValue)
            {
                var result = new decimal?[intoParts];
                var values = value.Value.Split(intoParts, roundingDecimals, roundingMode);
                for (int i = 0; i < intoParts; i++)
                {
                    result[i] = values[i];
                }
                return result;
            }
            else
            {
                return new decimal?[intoParts];
            }
        }

        /// <summary>
        /// Splits a given value into almost equal parts but such that all parts add up to the original value.
        /// Whenever the division as a modulus, that modulus is spread over the last returned values.
        /// I.e. 27 into 5 parts will result into 5, 5, 5, 6, 6.
        /// </summary>
        /// <param name="value">Value to split.</param>
        /// <param name="intoParts">Number of parts to split into (must be 1 or more).</param>
        /// <returns>Parts of the value.</returns>
        public static int[] Split(this int value, int intoParts)
        {
            if (intoParts <= 0) throw new ArgumentOutOfRangeException("intoParts", "The intoParts argument must not be less than 1.");

            if (value < 0)
            {
                var result = Split(-value, intoParts);
                for (int i = 0; i < result.Length; i++) result[i] = -result[i];
                return result;
            }
            else
            {
                var result = new int[intoParts];
                var part = value / intoParts;
                var modulus = value % intoParts;
                var partsNotToIncrease = intoParts - modulus;

                for (int i = 0; i < intoParts; i++)
                {
                    result[i] = part;
                    if (i >= partsNotToIncrease) result[i]++;
                }

                return result;
            }
        }

        /// <summary>
        /// Splits a given value into almost equal parts but such that all parts add up to the original value.
        /// Whenever the division has a modulus, that modulus is spread over the last returned values.
        /// I.e. 27 into 5 parts will result into 5, 5, 5, 6, 6.
        /// </summary>
        /// <param name="value">Value to split.</param>
        /// <param name="intoParts">Number of parts to split into (must be 1 or more).</param>
        /// <returns>Parts of the value.</returns>
        public static int?[] Split(this int? value, int intoParts)
        {
            if (value.HasValue)
            {
                var result = new int?[intoParts];
                var values = value.Value.Split(intoParts);
                for (int i = 0; i < intoParts; i++)
                {
                    result[i] = values[i];
                }
                return result;
            }
            else
            {
                return new int?[intoParts];
            }
        }
    }
}
