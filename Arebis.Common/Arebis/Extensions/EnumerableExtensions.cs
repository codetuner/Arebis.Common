using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the given enumerable values grouped in sets of groupSize.
        /// The last set can be smaller.
        /// </summary>
        /// <typeparam name="T">Type of values in enumeration.</typeparam>
        /// <param name="enumerable">An enumerable to group.</param>
        /// <param name="groupSize">Maximum group size. The last group can be smaller.</param>
        public static IEnumerable<T[]> GroupByCountOf<T>(this IEnumerable<T> enumerable, int groupSize)
        {
            using (var enumerator = enumerable.GetEnumerator())
            {
                var group = new T[groupSize];
                var groupIndex = 0;
                while (enumerator.MoveNext())
                {
                    group[groupIndex++] = enumerator.Current;
                    if (groupIndex == groupSize)
                    {
                        yield return group;
                        group = new T[groupSize];
                        groupIndex = 0;
                    }
                }

                if (groupIndex != 0)
                {
                    var remainder = new T[groupIndex];
                    Array.Copy(group, remainder, groupIndex);
                    yield return remainder;
                }
            }
        }

        /// <summary>
        /// Return the first index for which the given predicate matches. Returns -1 if no match was found.
        /// </summary>
        public static int IndexWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            var index = 0;
            foreach (var item in enumerable)
            {
                if (predicate(item)) return index;
                index++;
            }

            return -1;
        }

        /// <summary>
        /// Return the indexes for which the given predicate match.
        /// </summary>
        public static IEnumerable<int> IndexesWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            var index = 0;
            foreach (var item in enumerable)
            {
                if (predicate(item)) yield return index;
                index++;
            }
        }
    }
}
