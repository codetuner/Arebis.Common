using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Caching
{
    /// <summary>
    /// Defines behavior of a cache.
    /// </summary>
    public interface ICache<TKey, TValue>
    {
        /// <summary>
        /// Retrieves the value currently stored under the given key.
        /// Depending on the implementation, when no value is found under
        /// the given key, either returns default(TValue), or fetches, 
        /// stores and returns the requested value.
        /// </summary>
        TValue Get(TKey key);

        /// <summary>
        /// Stores a value under the given key. Overwrites previous
        /// values.
        /// </summary>
        void Set(TKey key, TValue value);

        /// <summary>
        /// Evicts the item currently stored in the cache under
        /// the given key. Returns the item that was stored, or default(TValue)
        /// if no value was stored.
        /// </summary>
        TValue Evict(TKey key);

        /// <summary>
        /// Evicts all values in the cache, clearing the cache.
        /// </summary>
        void EvictAll();
    }
}
