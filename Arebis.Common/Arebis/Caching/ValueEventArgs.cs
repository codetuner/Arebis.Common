using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Caching
{
    /// <summary>
    /// A generic EventArgs, containing a Value of type T.
    /// </summary>
    public class ValueEventArgs<T> : EventArgs
    {
        public ValueEventArgs(T item)
        {
            this.Item = item;
        }

        public T Item { get; private set; }
    }

    public class ResolveValueEventArgs<TKey, TValue> : EventArgs
    { 
        private TValue _value;

        public ResolveValueEventArgs(TKey key)
        {
            this.Key = key;
        }

        public TKey Key { get; private set; }

        public bool IsResolved { get; private set; }

        public TValue Value 
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                IsResolved = true;
            }
        }
    }
}
