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
