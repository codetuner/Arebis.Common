using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Extensions
{
    public static class CollectionExtension
    {
        public static void AddRange<TValue>(this ICollection<TValue> coll, IEnumerable<TValue> range)
        {
            foreach (var item in range)
                coll.Add(item);
        }

        public static string ToDictionaryString<TKey, TValue>(this IDictionary<TKey, TValue> dict, string keyPrefix = "", string keyValueSeparator = "=", string valueSuffix = "", string pairSeparator = "\r\n")
        {
            var builder = new StringBuilder();
            foreach (var key in dict.Keys.Select(k => Tuple.Create(k.ToString(), k)).OrderBy(k => k.Item1))
            {
                builder.Append(keyPrefix);
                builder.Append(key.Item1);
                builder.Append(keyValueSeparator);
                builder.Append(dict[key.Item2]);
                builder.Append(valueSuffix);
                builder.Append(pairSeparator);
            }

            builder.Length -= pairSeparator.Length;

            return builder.ToString();
        }

        public static void FillFromString(this IDictionary<string, string> dict, string stringDictionary, string keyPrefix = "", string keyValueSeparator = "=", string valueSuffix = "", string pairSeparator = "\r\n")
        {
            if (stringDictionary == null) return;

            var pairs = stringDictionary.SplitString(pairSeparator)
                .Where(p => p.Length >= (keyPrefix.Length + keyValueSeparator.Length + valueSuffix.Length))
                .Select(p => p.Substring(keyPrefix.Length, p.Length - -keyPrefix.Length - valueSuffix.Length))
                .Select(p => new KeyValuePair<string, string>(p.Substring(0, p.IndexOf(keyValueSeparator)), p.Substring(p.IndexOf(keyValueSeparator) + keyValueSeparator.Length)));

            foreach (var pair in pairs)
            {
                dict[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Removes several items from a collection.
        /// </summary>
        /// <returns>Values removed.</returns>
        public static IEnumerable<TValue> RemoveMany<TValue>(this ICollection<TValue> collection, IEnumerable<TValue> valuesToRemove)
        {
            var values = valuesToRemove.ToArray();
            foreach(var value in values)
            {
                collection.Remove(value);
            }

            return values;
        }

        /// <summary>
        /// Removes all items from a collection that match a given criteria.
        /// </summary>
        /// <returns>Values removed.</returns>
        public static IEnumerable<TValue> RemoveWhere<TValue>(this ICollection<TValue> collection, Func<TValue, bool> predicate)
        {
            return RemoveMany(collection, collection.Where(predicate));
        }

        /// <summary>
        /// Removes all occurences of the given item.
        /// </summary>
        /// <returns>The number of occurences removed.</returns>
        public static int RemoveAllOccurences<TValue>(this ICollection<TValue> collection, TValue item)
        {
            int count = 0;
            while (collection.Remove(item))
            {
                count++;
            }

            return count;
        }
    }
}
