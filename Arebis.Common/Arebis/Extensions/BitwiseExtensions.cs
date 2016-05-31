using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Extensions
{
    public static class BitwiseExtensions
    {
        /// <summary>
        /// Whether the given bit is set.
        /// </summary>
        public static bool IsBitSet(this System.Int32 bits, int bitNumber)
        {
            System.Int32 mask = 1 << bitNumber;
            return ((bits & mask) != 0);
        }

        /// <summary>
        /// Whether the given bit is set.
        /// </summary>
        [CLSCompliant(false)]
        public static bool IsBitSet(this System.UInt32 bits, int bitNumber)
        {
            System.UInt32 mask = ((System.UInt32)1) << bitNumber;
            return ((bits & mask) != 0);
        }

        /// <summary>
        /// Whether the given bit is set.
        /// </summary>
        public static bool IsBitSet(this System.Int64 bits, int bitNumber)
        {
            System.Int64 mask = 1L << bitNumber;
            return ((bits & mask) != 0);
        }

        /// <summary>
        /// Whether the given bit is set.
        /// </summary>
        [CLSCompliant(false)]
        public static bool IsBitSet(this System.UInt64 bits, int bitNumber)
        {
            System.UInt64 mask = ((System.UInt64)1L) << bitNumber;
            return ((bits & mask) != 0);
        }

        /// <summary>
        /// Sets the given bit to the given value (true=1, false=0).
        /// </summary>
        public static System.Int32 SetBit(this System.Int32 bits, int bitNumber, bool value)
        {
            System.Int32 mask = 1 << bitNumber;
            if (value == true)
                return (bits | mask);
            else
                return (bits & ~mask);
        }

        /// <summary>
        /// Sets the given bit to the given value (true=1, false=0).
        /// </summary>
        [CLSCompliant(false)]
        public static System.UInt32 SetBit(this System.UInt32 bits, int bitNumber, bool value)
        {
            System.UInt32 mask = ((System.UInt32)1) << bitNumber;
            if (value == true)
                return (bits | mask);
            else
                return (bits & ~mask);
        }

        /// <summary>
        /// Sets the given bit to the given value (true=1, false=0).
        /// </summary>
        public static System.Int64 SetBit(this System.Int64 bits, int bitNumber, bool value)
        {
            System.Int64 mask = 1 << bitNumber;
            if (value == true)
                return (bits | mask);
            else
                return (bits & ~mask);
        }

        /// <summary>
        /// Sets the given bit to the given value (true=1, false=0).
        /// </summary>
        [CLSCompliant(false)]
        public static System.UInt64 SetBit(this System.UInt64 bits, int bitNumber, bool value)
        {
            System.UInt64 mask = ((System.UInt64)1) << bitNumber;
            if (value == true)
                return (bits | mask);
            else
                return (bits & ~mask);
        }

        /// <summary>
        /// Enumerates the bit indexes of bits set to 1.
        /// </summary>
        public static IEnumerable<int> EnumerateBits(this System.Int32 bits)
        {
            System.Int32 mask = 1;
            for (int i = 0; i < 32; i++)
            {
                if ((bits & mask) != 0) yield return i;
                mask <<= 1;
            }
        }

        /// <summary>
        /// Enumerates the bit indexes of bits set to 1.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<int> EnumerateBits(this System.UInt32 bits)
        {
            System.UInt32 mask = (System.UInt32)1;
            for (int i = 0; i < 32; i++)
            {
                if ((bits & mask) != 0) yield return i;
                mask <<= 1;
            }
        }

        /// <summary>
        /// Enumerates the bit indexes of bits set to 1.
        /// </summary>
        public static IEnumerable<int> EnumerateBits(this System.Int64 bits)
        {
            System.Int64 mask = 1;
            for (int i = 0; i < 32; i++)
            {
                if ((bits & mask) != 0) yield return i;
                mask <<= 1;
            }
        }

        /// <summary>
        /// Enumerates the bit indexes of bits set to 1.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<int> EnumerateBits(this System.UInt64 bits)
        {
            System.UInt64 mask = (System.UInt64)1;
            for (int i = 0; i < 32; i++)
            {
                if ((bits & mask) != 0) yield return i;
                mask <<= 1;
            }
        }
    }
}
