using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Collections.Generic
{
    /// <summary>
    /// A DictionaryBag is a dictionary of bags. A dictionary where the key
    /// refers to a list of values.
    /// See also DictionaryOfLists.
    /// </summary>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of an individual value.</typeparam>
    [Serializable]
    public class DictionaryBag<TKey, TValue>
    {
        private Dictionary<TKey, List<TValue>> innerDictionary;

        /// <summary>
        /// Constructs a default DictionaryBag.
        /// </summary>
        public DictionaryBag()
        {
            this.innerDictionary = new Dictionary<TKey, List<TValue>>();
        }

        /// <summary>
        /// Constructs a DictionaryBag using the given key comparer.
        /// </summary>
        public DictionaryBag(IEqualityComparer<TKey> comparer)
        {
            this.innerDictionary = new Dictionary<TKey, List<TValue>>(comparer);
        }

        /// <summary>
        /// Constructs a DictionaryBag as shallow copy of the given
        /// original.
        /// </summary>
        public DictionaryBag(DictionaryBag<TKey, TValue> original)
        {
            this.innerDictionary = new Dictionary<TKey, List<TValue>>(original.innerDictionary);
        }

        /// <summary>
        /// Adds the given value under the given key.
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            List<TValue> valueList;
            if (!this.innerDictionary.TryGetValue(key, out valueList))
            {
                valueList = new List<TValue>();
                this.innerDictionary[key] = valueList;
            }
            valueList.Add(value);
        }

        /// <summary>
        /// Removes all values under the given key.
        /// </summary>
        public void Remove(TKey key)
        {
            this.innerDictionary.Remove(key);
        }

        /// <summary>
        /// Removes the given value under the given key.
        /// </summary>
        public void Remove(TKey key, TValue value)
        {
            List<TValue> valueList;
            if (this.innerDictionary.TryGetValue(key, out valueList))
                valueList.Remove(value);
        }

        /// <summary>
        /// Clears the whole DictionaryBag.
        /// </summary>
        public void Clear()
        {
            this.innerDictionary.Clear();
        }

        /// <summary>
        /// The number of keys.
        /// </summary>
        public int KeyCount
        {
            get { return this.innerDictionary.Count; }
        }

        /// <summary>
        /// The keys.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return this.innerDictionary.Keys; }
        }

        /// <summary>
        /// Whether the DictionaryBag contains the given key.
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            return this.innerDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Collection of values sharing the given key.
        /// </summary>
        public ICollection<TValue> this[TKey key]
        {
            get { return this.innerDictionary[key].AsReadOnly(); }
        }

        /// <summary>
        /// All values over all keys.
        /// </summary>
        public IEnumerable<TValue> AllValues
        {
            get 
            {
                foreach (KeyValuePair<TKey, List<TValue>> pair in this.innerDictionary)
                    foreach (TValue value in pair.Value)
                        yield return value;
            }
        }
    }
}
