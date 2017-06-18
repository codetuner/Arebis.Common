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
        /// Try to get the value matching the given key. If not found, returns the default value.
        /// </summary>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            TValue value;
            if (dict.TryGetValue(key, out value))
                return value;
            else
                return defaultValue;
        }

        /// <summary>
        /// If the value is it's types default (default(TValue)) then the key is removed
        /// from the dictionary (if present). Otherwise the key is added.
        /// </summary>
        /// <typeparam name="TKey">Type of the Key.</typeparam>
        /// <typeparam name="TValue">Type of the Value (must be a reference type).</typeparam>
        /// <param name="dict">Doctionary.</param>
        /// <param name="key">Key to set value for.</param>
        /// <param name="value">Value to set.</param>
        /// <returns>The value set.</returns>
        public static TValue SetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
            where TValue : class
        {
            if (value == default(TValue))
            {
                dict.Remove(key);
            }
            else
            {
                dict[key] = value;
            }

            return value;
        }

        /// <summary>
        /// For a dictionary where the value is a list of, adds an element to the list matching the given key.
        /// If no list of the key exists yet, a list is created and the value is added to it.
        /// </summary>
        /// <typeparam name="TKey">Type of key of the dictionary.</typeparam>
        /// <typeparam name="TValue">Type of value of the lists in the dictionary.</typeparam>
        /// <param name="dict">The dictionary.</param>
        /// <param name="key">The key to identify the list.</param>
        /// <param name="value">The value to add to the list.</param>
        /// <returns>The list where the value was added to.</returns>
        public static IList<TValue> AddListValue<TKey, TValue>(this IDictionary<TKey, IList<TValue>> dict, TKey key, TValue value)
        {
            IList<TValue> list;
            if (!dict.TryGetValue(key, out list))
            {
                dict[key] = list = new List<TValue>();
            }

            list.Add(value);

            return list;
        }

        /// <summary>
        /// Try to get the value matching the given key. If not found, executes the value function, then stores and returns the result.
        /// </summary>
        /// <example>
        /// <code>var be = countries.GetOrCacheValue("BE", () =&gt; dbContext.Countries.Single(c => c.CountryCode == "BE"));</code>
        /// </example>
        public static TValue GetOrCacheValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> valueFunction)
        {
            TValue value;
            if (dict.TryGetValue(key, out value))
                return value;
            else
                return dict[key] = valueFunction();
        }

        /// <summary>
        /// Adds/overwrites the given key/value to the dictionary and returns the dictionary for fluent syntax.
        /// </summary>
        public static T With<T, K, V>(this T dict, K key, V value)
            where T : IDictionary<K, V>
        {
            dict[key] = value;
            return dict;
        }

        /// <summary>
        /// Removes the given key from the dictionary (if present) and returns the dictionary for fluent syntax.
        /// </summary>
        public static IDictionary<TKey, TValue> Without<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            dict.Remove(key);
            return dict;
        }

        /// <summary>
        /// Merges the value of the other dictionary in this one.
        /// </summary>
        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> other)
        {
            foreach (var pair in other)
            {
                dict[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Converts this dictionary to a string that could be parsed to rebuild the dictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="keyPrefix"></param>
        /// <param name="keyValueSeparator"></param>
        /// <param name="valueSuffix"></param>
        /// <param name="pairSeparator"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Fills a string dictionary with content parsed from a given string.
        /// </summary>
        /// <param name="dict">The dictionary object to fill.</param>
        /// <param name="stringDictionary">The string to parse and take content from.</param>
        /// <param name="keyPrefix">Character sequence to recognize as a key prefix.</param>
        /// <param name="keyValueSeparator">Character sequence separating keys from values.</param>
        /// <param name="valueSuffix">Character sequence suffixing values.</param>
        /// <param name="pairSeparator">Character sequence separating key/value pairs.</param>
        /// <example>dict.FillFromString("'name'='John';'city'='LA'", "'", "'='", "'", ";");</example>
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
        /// Returns a new dictionary with keys and values transformed according to transformation functions.
        /// </summary>
        /// <typeparam name="TKeyIn">Type of the input directory keys.</typeparam>
        /// <typeparam name="TKeyOut">Type of the resulting dictionary keys.</typeparam>
        /// <typeparam name="TValueIn">Type of the input dictionary values.</typeparam>
        /// <typeparam name="TValueOut">Type of the resulting dictionary values.</typeparam>
        /// <param name="dict">Input dictionary to transform.</param>
        /// <param name="keyTransformation">Transformation function for the keys.</param>
        /// <param name="valueTransformation">Transformation function for the values.</param>
        /// <returns>A dictionary with transformed items.</returns>
        public static IDictionary<TKeyOut, TValueOut> Transform<TKeyIn, TKeyOut, TValueIn, TValueOut>(this IEnumerable<KeyValuePair<TKeyIn, TValueIn>> dict, Func<KeyValuePair<TKeyIn, TValueIn>, TKeyOut> keyTransformation, Func<KeyValuePair<TKeyIn, TValueIn>, TValueOut> valueTransformation)
        { 
            var result = new Dictionary<TKeyOut, TValueOut>();

            foreach (var item in dict)
            {
                result[keyTransformation(item)] = valueTransformation(item);
            }

            return result;
        }

    }
}
