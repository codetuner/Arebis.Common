using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Removes the first element. Returns false if list is empty.
        /// </summary>
        public static bool RemoveFirst<T>(this IList<T> list)
        {
            var lc = list.Count;
            if (lc == 0) return false;
            list.RemoveAt(0);
            return true;
        }

        /// <summary>
        /// Removes the last element. Returns false if list is empty.
        /// </summary>
        public static bool RemoveLast<T>(this IList<T> list)
        {
            var lc = list.Count;
            if (lc == 0) return false;
            list.RemoveAt(lc - 1);
            return true;
        }
    }
}
