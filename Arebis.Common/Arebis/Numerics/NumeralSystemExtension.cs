using Arebis.Numerics;
using Arebis.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Numerics
{
    public static class NumeralSystemExtension
    {
        public static BigInteger ParseBigInteger(this NumeralSystem system, string s)
        {
            s = system.PrepareForParse(s);

            var result = new BigInteger();
            foreach (var c in s)
            {
                var value = system.GetValueForSymbol(c);
                if (value == -1) continue;
                result = (result * system.Base) + value;
            }
            return result;
        }

        public static string FromBigInteger(this NumeralSystem system, BigInteger value, int fixedLength = 0, int groupSize = 0, string groupSeparator = " ")
        {
            var chars = new char[20];
            var charcount = 0;

            do
            {
                chars[chars.Length - charcount - 1] = system.GetSymbolFor((int)(value % system.Base));
                value = value / system.Base;
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
                result = new String(system.GetSymbolFor(0), fixedLength - result.Length) + result;
            }
            if (result.Length > groupSize && groupSize > 0 && !String.IsNullOrEmpty(groupSeparator))
            {
                return String.Join(groupSeparator, result.Chunked(groupSize, true));
            }

            return result;
        }

        public static BigInteger ToBigInteger(this NumeralValue value)
        {
            return value.System.ParseBigInteger(value.Value);
        }
    }
}
