using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Algorithms
{
    /// <summary>
    /// Collection of static enumerator functions.
    /// The returned enumerations can be combined with Linq methods.
    /// </summary>
    public static class Enumerate
    {
        /// <summary>
        /// Enumerates all integer values from-to the given values.
        /// I.e: Enumerate.FromTo(0, 3) will enumerate 0, 1, 2 and 3.
        /// I.e: Enumerate.FromTo(0, 20, 5) will enumerate 0, 5, 10, 15, 20.
        /// </summary>
        public static IEnumerable<int> FromTo(int from, int upToIncluded, int stepSize = 1)
        {
            for (int i = from; i <= upToIncluded; i += stepSize)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Enumerates all DateTime values from-to the given values.
        /// </summary>
        public static IEnumerable<DateTime> FromTo(DateTime from, DateTime upToIncluded, TimeSpan stepSize)
        {
            var i = from;
            while (i <= upToIncluded)
            {
                yield return i;
                i += stepSize;
            }
        }

        /// <summary>
        /// Powers of the given number (n^0, n^1, n^2, n^3,...).
        /// </summary>
        public static IEnumerable<double> PowersOf(double number, double upToIncluded)
        {
            var i = 0.0;
            while (true)
            {
                var item = Math.Pow(number, i);
                if (item > upToIncluded) break;
                yield return item;
                i += 1.0;
            }
        }

        /// <summary>
        /// Enumerates the Fibonacci sequence.
        /// </summary>
        public static IEnumerable<long> Fibonacci(long upToIncluded)
        {
            if (upToIncluded >= 0L) yield return 0L;
            if (upToIncluded >= 1L) yield return 1L;
            var p = 0L;
            var q = 1L;
            while (true)
            {
                var item = p + q;
                if (item > upToIncluded) break;
                yield return item;
                p = q;
                q = item;
            }
        }
    }
}
