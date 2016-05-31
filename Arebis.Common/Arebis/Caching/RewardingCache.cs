using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arebis.Caching
{
    /// <summary>
    /// A RewardingCache is a cache that rewards items when hitted
    /// by upgrading their position in the cache.
    /// </summary>
    public class RewardingCache<TKey, TValue>
        where TValue : class
    {
        private int _maxSize;
        private int _hitBenefit;
        private int _entrance;

        private LinkedList<KeyValuePair<TKey, TValue>> _cacheStore = new LinkedList<KeyValuePair<TKey, TValue>>();
        private Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _cacheIndex = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
        private Object _syncRoot = new Object();
        private int _count = 0;

        #region Constructors

        /// <summary>
        /// Constructs a default RewardingCache instance sized for 100 elements.
        /// </summary>
        public RewardingCache()
            : this(100, 10, 50)
        { }

        ///// <summary>
        ///// Constructs a new RewardingCache instance based on configuration information.
        ///// </summary>
        //public RewardingCache(string configurationSection)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Constructs a new RewardingCache instance.
        /// </summary>
        /// <param name="maxSize">Maximum number of elements stored in the cache.</param>
        /// <param name="hitBenefit">Benefit (=number of positions) an elmement gets when hitted.</param>
        /// <param name="entrance">Entrance position for new elements.</param>
        public RewardingCache(int maxSize, int hitBenefit, int entrance)
        {
            // Validate arguments:
            if (maxSize <= 0) throw new ArgumentOutOfRangeException("maxSize");
            if (hitBenefit < 0) throw new ArgumentOutOfRangeException("hitBenefit");
            if (hitBenefit > maxSize) throw new ArgumentOutOfRangeException("hitBenefit", "The hitBenefit argument cannot be larger than maxSize.");
            if (entrance < 0) throw new ArgumentOutOfRangeException("entrance");
            if (entrance >= maxSize) throw new ArgumentOutOfRangeException("entrance", "The entrance argument must be less thant maxSize.");

            // Configure instance:
            _maxSize = maxSize;
            _hitBenefit = hitBenefit;
            _entrance = entrance;
        }

        #endregion Constructors

        #region Public API

        /// <summary>
        /// Handler to resolve items not found in the cache.
        /// </summary>
        public event EventHandler<ResolveValueEventArgs<TKey, TValue>> ResolveEventHandler;

        /// <summary>
        /// Triggered when the cache is entirely invalidated.
        /// </summary>
        public event EventHandler CacheInvalidated;

        /// <summary>
        /// Triggered when a new item is added to the cache.
        /// </summary>
        public event EventHandler<ValueEventArgs<KeyValuePair<TKey, TValue>>> CacheAdded;

        /// <summary>
        /// Triggered when an item is removed from the cache.
        /// </summary>
        public event EventHandler<ValueEventArgs<KeyValuePair<TKey, TValue>>> CacheRemoved;

        /// <summary>
        /// An item in the cache.
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                lock (_syncRoot)
                {
                    // If item in cache:
                    LinkedListNode<KeyValuePair<TKey, TValue>> node;
                    if (_cacheIndex.TryGetValue(key, out node))
                    {

                        // Item is hitted and must get benefit:
                        // 1) Find new location:
                        LinkedListNode<KeyValuePair<TKey, TValue>> prevNode = node.Previous;
                        for (int i = 0; i < _hitBenefit; i++)
                        {
                            if (prevNode == null) break;
                            prevNode = prevNode.Previous;
                        }
                        // 2) Remove node from old location:
                        _cacheStore.Remove(node);
                        // 3) Insert node at new location:
                        if (prevNode == null)
                            _cacheStore.AddFirst(node);
                        else
                            _cacheStore.AddAfter(prevNode, node);

                        // Return value:
                        return node.Value.Value;
                    }
                    else
                    {
                        if (this.ResolveEventHandler != null)
                        {
                            // Resolve item, store it and return it:
                            var e = new ResolveValueEventArgs<TKey, TValue>(key);
                            this.ResolveEventHandler(this, e);
                            if (e.IsResolved)
                            {
                                return (this[key] = e.Value);
                            }
                        }

                        // Otherwise, return default value:
                        return default(TValue);
                    }
                }
            }
            set
            {
                lock (_syncRoot)
                {
                    LinkedListNode<KeyValuePair<TKey, TValue>> node;
                    if (_cacheIndex.TryGetValue(key, out node))
                    {
                        // Overwrite value in cache:
                        node.Value = new KeyValuePair<TKey, TValue>(key, value);
                    }
                    else
                    {
                        // If cache is full, remove last item:
                        if (_count >= _maxSize)
                        {
                            LinkedListNode<KeyValuePair<TKey, TValue>> lastNode = _cacheStore.Last;
                            _cacheIndex.Remove(lastNode.Value.Key);
                            _cacheStore.RemoveLast();
                            _count--;
                            this.OnCacheRemoved(lastNode.Value);
                        }

                        // Add new item:
                        if (_entrance == 0)
                        {
                            // Add node:
                            node = _cacheIndex[key] = _cacheStore.AddFirst(new KeyValuePair<TKey, TValue>(key, value));
                        }
                        else
                        {
                            // Navigate to entry position:
                            LinkedListNode<KeyValuePair<TKey, TValue>> prevNode = _cacheStore.First;
                            for (int i = 1; i < _entrance; i++)
                            {
                                if (prevNode == null) break;
                                prevNode = prevNode.Next;
                            }

                            // Add node:
                            if (prevNode == null)
                                node = _cacheIndex[key] = _cacheStore.AddLast(new KeyValuePair<TKey, TValue>(key, value));
                            else
                                node = _cacheIndex[key] = _cacheStore.AddAfter(prevNode, new KeyValuePair<TKey, TValue>(key, value));
                        }

                        // Increase count:
                        _count++;

                        // Send event:
                        this.OnCacheAdded(node.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Invalidate the given item in the cache.
        /// </summary>
        public void Invalidate(TKey key)
        {
            lock (_syncRoot)
            {
                LinkedListNode<KeyValuePair<TKey, TValue>> node;
                if (_cacheIndex.TryGetValue(key, out node))
                {
                    _cacheIndex.Remove(key);
                    _cacheStore.Remove(node);
                    _count--;
                    this.OnCacheRemoved(node.Value);
                }
            }
        }

        /// <summary>
        /// Invalidate all items in the cache.
        /// </summary>
        public void InvalidateAll()
        {
            lock (_syncRoot)
            {
                _cacheIndex = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
                _cacheStore = new LinkedList<KeyValuePair<TKey, TValue>>();
                _count = 0;
                this.OnCacheInvalidated();
            }
        }

        /// <summary>
        /// Root synchronisation object.
        /// </summary>
        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        public CacheInfo CacheInfo {
            get { 
                return new CacheInfo() {
                    Count= this._count,
                    Entrance= _entrance,
                    HitBenefit= _hitBenefit,
                    MaxSize= _maxSize
                };
            }
        }

       


        #endregion Public API

        #region Internal API

        /// <summary>
        /// Cache is invalided
        /// </summary>
        protected virtual void OnCacheInvalidated()
        {
            if (this.CacheAdded != null)
                this.CacheInvalidated(this, EventArgs.Empty);
        }

        /// <summary>
        /// Cache item added
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnCacheAdded(KeyValuePair<TKey, TValue> item)
        {
            if (this.CacheAdded != null)
                this.CacheAdded(this, new ValueEventArgs<KeyValuePair<TKey, TValue>>(item));
        }

        /// <summary>
        /// Cache item removed
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnCacheRemoved(KeyValuePair<TKey, TValue> item)
        {
            if (this.CacheRemoved != null)
                this.CacheRemoved(this, new ValueEventArgs<KeyValuePair<TKey, TValue>>(item));
        }

        #endregion Internal API

        #region API for testing

        /// <summary>
        /// Returns the keys/values of the element in order of priority.
        /// First element is most often hitted and least to be removed from the cache.
        /// </summary>
        internal KeyValuePair<TKey, TValue>[] Content
        {
            get
            {
                lock (_syncRoot)
                {
                    return _cacheStore.ToArray();
                }
            }
        }

        #endregion
    }


    public class CacheInfo
    {
        public int MaxSize { get; internal set; }
        public int HitBenefit { get; internal set; }
        public int Entrance { get; internal set; }
        public int Count { get; internal set; }
        //public CacheStatus(int maxSize, int hitBenefit, int entrace, int count)
        //{
        //    MaxSize= maxSize;
        //    HitBenefit= hitBenefit;
        //    Entrance= entrace;
        //    Count= count;
        //}
    }
}
