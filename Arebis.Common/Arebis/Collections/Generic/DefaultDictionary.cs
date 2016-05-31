using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Collections.Generic
{
    [Serializable]
    public class DefaultDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> internalDictionary;

        public DefaultDictionary()
        {
            this.internalDictionary = new Dictionary<TKey, TValue>();
        }

        public DefaultDictionary(IEqualityComparer<TKey> comparer)
        {
            this.internalDictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public DefaultDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.internalDictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public DefaultDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.internalDictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            this.internalDictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return this.internalDictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return this.internalDictionary.Keys; }
        }

        public bool Remove(TKey key)
        {
            return this.internalDictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.internalDictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return this.internalDictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (this.internalDictionary.TryGetValue(key, out value))
                    return value;
                else
                    return default(TValue);
            }
            set
            {
                this.internalDictionary[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        void ICollection<KeyValuePair<TKey,TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey,TValue>>)this.internalDictionary).Add(item);
        }

        public void Clear()
        {
            this.internalDictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.internalDictionary.Contains(item);
        }

        void ICollection<KeyValuePair<TKey,TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey,TValue>>)this.internalDictionary).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.internalDictionary.Count; }
        }

        bool ICollection<KeyValuePair<TKey,TValue>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<TKey,TValue>>)this.internalDictionary).IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.internalDictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)this.GetEnumerator();
        }

        #endregion
    }
}
