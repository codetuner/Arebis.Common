using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Extensions
{
    public static class BitArrayExtensions
    {
        public static Byte[] ToByteArray(this BitArray array)
        {
            var result = new Byte[(int)((array.Length + 7) / 8)];
            array.CopyTo(result, 0);
            return result;
        }

        public static Int32[] ToInt32Array(this BitArray array)
        {
            var result = new Int32[(int)((array.Length + 31) / 32)];
            array.CopyTo(result, 0);
            return result;
        }

        public static IEnumerable<int> EnumerateBitsSet(this BitArray array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array.Get(i)) yield return i;
            }
        }

        public static void SetBits(this BitArray array, int[] bitIndexes)
        {
            foreach (var i in bitIndexes)
            {
                array.Set(i, true);
            }
        }
    }
}
