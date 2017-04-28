using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Numerics
{
    /// <summary>
    /// A value expressed in a numeral system.
    /// </summary>
    [DataContract]
    [Serializable]
    public class NumeralValue : IConvertible, IComparable<NumeralValue>, IComparable, IEquatable<NumeralValue>, IFormattable
    {
        /// <summary>
        /// Creates a numeral value.
        /// </summary>
        public NumeralValue(NumeralSystem system, long value)
            : this(system, system.From(value))
        { }

        /// <summary>
        /// Creates a numeral value.
        /// </summary>
        public NumeralValue(NumeralSystem system, string value)
        {
            this.Value = value;
            this.System = system;
        }

        /// <summary>
        /// The raw value of this NumeralValue.
        /// </summary>
        [DataMember]
        public String Value { get; private set; }

        /// <summary>
        /// The numeral system of this NumeralValue.
        /// </summary>
        [DataMember]
        public NumeralSystem System { get; private set; }

        /// <summary>
        /// Returns a string representation of this NumeralValue.
        /// </summary>
        public override string ToString()
        {
            return this.System.ValueToString(this.Value);
        }

        #region IConvertible implementation

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(this.System.ParseInt64(this.Value));
        }

        public byte ToByte(IFormatProvider provider)
        {
            return this.System.ParseByte(this.Value);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this.System.ParseInt64(this.Value));
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(this.System.ParseInt64(this.Value));
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(this.System.ParseInt64(this.Value));
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(this.System.ParseInt64(this.Value));
        }

        public short ToInt16(IFormatProvider provider)
        {
            return this.System.ParseInt16(this.Value);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return this.System.ParseInt32(this.Value);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return this.System.ParseInt64(this.Value);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this.System.ParseInt64(this.Value));
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this.System.ParseInt64(this.Value));
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return this.ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(this.System.ParseInt64(this.Value), conversionType);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this.System.ParseInt64(this.Value));
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this.System.ParseInt64(this.Value));
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this.System.ParseInt64(this.Value));
        }

        #endregion

        #region IComparable<> and IComparable implementations

        public int CompareTo(NumeralValue other)
        {
            var tbd = this.System.ParseBigInteger(this.Value);
            var obd = other.System.ParseBigInteger(other.Value);
            return tbd.CompareTo(obd);
        }

        int IComparable.CompareTo(object obj)
        {
            var other = obj as NumeralValue;
            if (other == null) return 1;
            var tbd = this.System.ParseBigInteger(this.Value);
            var obd = other.System.ParseBigInteger(other.Value);
            return tbd.CompareTo(obd);
        }

        #endregion

        #region IEquatable<> implementation

        public bool Equals(NumeralValue other)
        {
            var tbd = this.System.ParseBigInteger(this.Value);
            var obd = other.System.ParseBigInteger(other.Value);
            return tbd.Equals(obd);
        }

        #endregion

        #region IFormattable implementation

        /// <summary>
        /// Formats this value using a formatstring with [FixedLength[;GroupSize[;GroupSeparator]]].
        /// I.e. with format "12;4;-" a value will be formatted with fixed length of 12, in groups of 4 with "-" as group separator.
        /// </summary>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (String.IsNullOrWhiteSpace(format))
            {
                return this.ToString();
            }
            else
            {
                var parts = format.Split(';');
                var fl = 0; if (parts.Length > 0 && parts[0].Length > 0) fl = Int32.Parse(parts[0]);
                var gs = 0; if (parts.Length > 1 && parts[1].Length > 0) gs = Int32.Parse(parts[1]);
                var sp = " "; if (parts.Length > 2) sp = parts[2];
                return this.System.FromBigInteger(this.System.ParseBigInteger(this.Value), fl, gs, sp);
            }
        }

        #endregion
    }
}
