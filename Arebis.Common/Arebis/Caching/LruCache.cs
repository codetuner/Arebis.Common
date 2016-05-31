using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arebis.Caching
{
    /// <summary>
    /// A thread-safe LRU cache implementation. When the cache capacity is
    /// reached, Least Recently Used items are removed.
    /// </summary>
    public class LruCache<TKey, TValue> : ICache<TKey, TValue>
        where TValue : class
    {
        private readonly Object SyncRoot = new Object();
        private Dictionary<TKey, TValue> index = new Dictionary<TKey, TValue>();
        private LinkedList<TKey> list = new LinkedList<TKey>();

        /// <summary>
        /// The maximum number of items allowed in the cache.
        /// </summary>
        public int ItemCountLimit { get; private set; }

        /// <summary>
        /// The current number of items in the cache.
        /// </summary>
        public int CurrentItemCount { get; private set; }

        /// <summary>
        /// Instantiates a new LruCache.
        /// ItemCountLimit is set by the "{name}.ItemCountLimit" AppSetting.
        /// </summary>
        /// <param name="name">Name of the cache, used as base name for AppSetting configuration.</param>
        protected LruCache(string name)
        {
            this.ItemCountLimit = Int32.Parse(ConfigurationManager.AppSettings[name + ".ItemCountLimit"] ?? Int32.MaxValue.ToString());
        }

        /// <summary>
        /// Instantiates a new LruCache.
        /// </summary>
        /// <param name="itemCountLimit">The maximum number of items the cache is allowed to contain.</param>
        protected LruCache(int itemCountLimit)
        {
            this.ItemCountLimit = itemCountLimit;
        }

        /// <summary>
        /// Retrieves the value currently stored under the given key.
        /// When no value is found under the given key, tries to fetch,
        /// store and return the requested value.
        /// </summary>
        public virtual TValue Get(TKey key)
        {
            TValue value;

            lock (this.SyncRoot)
            {
                if (index.TryGetValue(key, out value))
                {
                    // Check item validity:
                    if (CheckValidity(value))
                    {
                        // Requeue the item:
                        Requeue(key, value);
                    }
                    else
                    {
                        // Evict the item:
                        Evict(key);
                        value = default(TValue);
                    }
                }

                if (value == default(TValue))
                {
                    value = TryFetch(key);
                    if (value != default(TValue))
                    {
                        // Adds the fetched item:
                        Add(key, value);
                    }
                }
            }

            // Return the value:
            return value;
        }

        /// <summary>
        /// Retrieves the value currently stored under the given key.
        /// Does not try to fetch missing items, but returns default(TValue).
        /// </summary>
        public TValue GetNoFetch(TKey key)
        {
            TValue value;

            lock (this.SyncRoot)
            {
                if (index.TryGetValue(key, out value))
                {
                    // Check item validity:
                    if (CheckValidity(value))
                    {
                        // Requeue the item:
                        Requeue(key, value);
                    }
                    else
                    {
                        // Evict the item:
                        Evict(key);
                        value = default(TValue);
                    }
                }
            }

            // Return the value:
            return value;
        }

        /// <summary>
        /// Stores a value under the given key. Evicts previous
        /// values.
        /// </summary>
        public virtual void Set(TKey key, TValue value)
        {
            lock (this.SyncRoot)
            {
                TValue previousValue;
                if (index.TryGetValue(key, out previousValue))
                {
                    Evict(key);
                }

                Add(key, value);
            }
        }

        /// <summary>
        /// Evicts the item currently stored in the cache under
        /// the given key. Returns the item that was stored, or default(TValue)
        /// if no value was stored.
        /// </summary>
        public virtual TValue Evict(TKey key)
        {
            lock (this.SyncRoot)
            {
                TValue value;
                if (index.TryGetValue(key, out value))
                {
                    this.index.Remove(key);
                    this.list.Remove(key);
                    this.CurrentItemCount--;
                    
                    this.OnEvicted(key, value);

                    return value;
                }
                else
                {
                    return default(TValue);
                }
            }
        }

        /// <summary>
        /// Evicts all values in the cache, clearing the cache.
        /// </summary>
        public virtual void EvictAll()
        {
            lock (this.SyncRoot)
            {
                this.index.Clear();
                this.list.Clear();
                this.CurrentItemCount = 0;
                this.OnEvictedAll();
            }
        }

        /// <summary>
        /// Fetches the requested value. Returns default(TValue)
        /// if not found.
        /// </summary>
        /// <remarks>
        /// While fetching, a lock is held on the whole cache, therefore
        /// fetching should be a fast operation. Consider using lazy
        /// initalization of the fetched object to increase it's speed
        /// taking thread safety and exception handling into account.
        /// </remarks>
        protected virtual TValue TryFetch(TKey key)
        {
            return default(TValue);
        }

        /// <summary>
        /// Returns true if the given value is still valid.
        /// </summary>
        /// <remarks>
        /// During the operation of this method the whole cache is locked.
        /// </remarks>
        public virtual bool CheckValidity(TValue value)
        {
            return true;
        }

        /// <summary>
        /// Adds a nonexisting item to the cache.
        /// </summary>
        /// <remarks>
        /// During the operation of this method the whole cache is locked.
        /// </remarks>
        protected virtual void Add(TKey key, TValue value)
        {
            // Adds the item to both the index and the list:
            index.Add(key, value);
            list.AddFirst(key);

            // Increase the current item count:
            CurrentItemCount++;

            this.OnAdded(key, value);

            // Reduce the cache size if too large:
            while (this.IsTooLarge())
            {
                this.ReduceSize();
            }
        }

        /// <summary>
        /// Returns true if the cache is too large.
        /// </summary>
        /// <remarks>
        /// During the operation of this method the whole cache is locked.
        /// </remarks>
        protected virtual bool IsTooLarge()
        {
            return (this.CurrentItemCount > this.ItemCountLimit);
        }

        /// <summary>
        /// Reduces the size of the cache.
        /// </summary>
        /// <remarks>
        /// During the operation of this method the whole cache is locked.
        /// </remarks>
        protected virtual void ReduceSize()
        { 
            // Remove the last item of the cache:
            this.Evict(list.Last.Value);
        }

        /// <summary>
        /// Repositions an item found in the cache, on top of the list.
        /// </summary>
        /// <remarks>
        /// During the operation of this method the whole cache is locked.
        /// </remarks>
        protected virtual void Requeue(TKey key, TValue value)
        {
            // Removes the item from the list, and add it in front:
            list.Remove(key);
            list.AddFirst(key);
        }

        /// <summary>
        /// Called whenever an item is added to the cache.
        /// </summary>
        /// <remarks>
        /// During the operation of this method the whole cache is locked.
        /// </remarks>
        protected virtual void OnAdded(TKey key, TValue value)
        {
        }

        /// <summary>
        /// Called whenever an individual item is evicted from the cache.
        /// </summary>
        /// <remarks>
        /// During the operation of this method the whole cache is locked.
        /// </remarks>
        protected virtual void OnEvicted(TKey key, TValue value)
        {
        }

        /// <summary>
        /// Called whenever all items are avicted.
        /// </summary>
        /// <remarks>
        /// During the operation of this method the whole cache is locked.
        /// </remarks>
        protected virtual void OnEvictedAll()
        { 
        }
    }
}
