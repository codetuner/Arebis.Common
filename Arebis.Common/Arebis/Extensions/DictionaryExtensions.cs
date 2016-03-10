using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Sets all values of the otherDict to the current one, effectively merging the otherDict with the current one.
        /// The otherDict can overwrite values already present in the current one.
        /// </summary>
        public static void SetAll<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> otherDict)
        {
            foreach (var pair in otherDict)
            {
                dict[pair.Key] = pair.Value;
            }
        }
    }
}
