using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arebis.Extensions;
using System.Numerics;
using System.Runtime.Serialization;

namespace Arebis.Numerics
{
    /// <summary>
    /// Represents a numeral system of arbitrary base.
    /// </summary>
    [DataContract]
    [Serializable]
    public class NumeralSystem
    {
        private static Random rnd = new Random();

        /// <summary>
        /// Creates a numeral system with given base.
        /// </summary>
        public NumeralSystem(int @base)
            : base()
        {
            this.Base = @base;
        }

        [DataMember]
        public int Base { get; private set; }

        public byte ParseByte(string s)
        {
            s = PrepareForParse(s);

            var result = (byte)0;
            foreach (var c in s)
            {
                var value = this.GetValueForSymbol(c);
                if (value == -1) continue;
                result = (byte)((result * this.Base) + value);
            }
            return result;
        }

        public short ParseInt16(string s)
        {
            s = PrepareForParse(s);

            var result = (short)0;
            foreach (var c in s)
            {
                var value = this.GetValueForSymbol(c);
                if (value == -1) continue;
                result = (short)((result * this.Base) + value);
            }
            return result;
        }

        public int ParseInt32(string s)
        {
            s = PrepareForParse(s);

            var result = 0;
            foreach (var c in s)
            {
                var value = this.GetValueForSymbol(c);
                if (value == -1) continue;
                result = (result * this.Base) + value;
            }
            return result;
        }

        public long ParseInt64(string s)
        {
            s = PrepareForParse(s);

            var result = 0L;
            foreach (var c in s)
            {
                var value = this.GetValueForSymbol(c);
                if (value == -1) continue;
                result = (result * this.Base) + value;
            }
            return result;
        }

        /// <summary>
        /// Converts the given value to the current base.
        /// </summary>
        public string From(long value, int fixedLength = 0, int groupSize = 0, string groupSeparator = " ")
        {
            var chars = new char[20];
            var charcount = 0;

            do
            {
                chars[chars.Length - charcount - 1] = this.GetSymbolFor((int)(value % this.Base));
                value = value / this.Base;
                charcount++;
                if (charcount >= chars.Length)
                {
                    var oldchars = chars;
                    chars = new char[oldchars.Length + 30];
                    Array.Copy(oldchars, 0, chars, 30, oldchars.Length);
                }
            } while (value > 0);

            var result = new String(chars, chars.Length - charcount, charcount);
            if (result.Length < fixedLength)
            {
                result = new String(this.GetSymbolFor(0), fixedLength - result.Length) + result;
            }
            if (result.Length > groupSize && groupSize > 0 && !String.IsNullOrEmpty(groupSeparator))
            {
                return String.Join(groupSeparator, result.Chunked(groupSize, true));
            }

            return result;
        }

        /// <summary>
        /// Generates a random sequence of length symbols within the current numeral system.
        /// </summary>
        public string Random(int length, int groupSize = 0, string groupSeparator = " ")
        {
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = this.GetSymbolFor(rnd.Next(this.Base));
            }

            if (groupSize <= 0 || groupSize >= length)
                return new String(result);
            else
                return String.Join(groupSeparator, new String(result).Chunked(groupSize, true));
        }

        /// <summary>
        /// Converts a value expressed in the current numeral system into another numeral system. Can handle values up to Int64.MaxValue.
        /// </summary>
        /// <param name="targetNumeralSystem">The numeral system to convert to.</param>
        /// <param name="value">The value expressed in the current numeral system.</param>
        /// <param name="fixedLength">If given, result will have at least the given length, padded with '0' symbols on the left.</param>
        /// <param name="groupSize">To group symbols together, give a groupSize.</param>
        /// <param name="groupSeparator">String to separate groups.</param>
        /// <returns>The value converted to the target numeral system.</returns>
        public string ConvertTo(NumeralSystem targetNumeralSystem, string value, int fixedLength = 0, int groupSize = 0, string groupSeparator = " ")
        {
            return targetNumeralSystem.From(this.ParseInt64(value), fixedLength, groupSize, groupSeparator);            
        }

        /// <summary>
        /// Converts a value expressed in the current numeral system into another numeral system. Can handle values of any size.
        /// </summary>
        /// <param name="targetNumeralSystem">The numeral system to convert to.</param>
        /// <param name="value">The value expressed in the current numeral system.</param>
        /// <param name="fixedLength">If given, result will have at least the given length, padded with '0' symbols on the left.</param>
        /// <param name="groupSize">To group symbols together, give a groupSize.</param>
        /// <param name="groupSeparator">String to separate groups.</param>
        /// <returns>The value converted to the target numeral system.</returns>
        public string ConvertBigTo(NumeralSystem targetNumeralSystem, string value, int fixedLength = 0, int groupSize = 0, string groupSeparator = " ")
        {
            return targetNumeralSystem.FromBigInteger(this.ParseBigInteger(value), fixedLength, groupSize, groupSeparator);
        }

        public virtual char GetSymbolFor(int value)
        {
            return (char)value;
        }

        public virtual int GetValueForSymbol(char symbol)
        {
            return (int)symbol;
        }

        public virtual string PrepareForParse(string s)
        {
            return s;
        }

        /// <summary>
        /// Returns a string representation of the given value expressed in this numeral system.
        /// </summary>
        public virtual string ValueToString(string value)
        { 
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }
    }
}
