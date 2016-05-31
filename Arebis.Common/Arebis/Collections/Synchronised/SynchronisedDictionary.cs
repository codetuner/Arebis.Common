using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;

namespace Arebis.Collections.Synchronised
{
	/// <summary>
	/// Wrapper for synchronised access to an inner dictionary.
	/// </summary>
	[Serializable]
	public class SynchronisedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDeserializationCallback
	{
		private IDictionary<TKey, TValue> innerDictionary;
		[NonSerialized] private ReaderWriterLock syncLock = new ReaderWriterLock();
		private volatile int lockTimeout = 10000;
		
		#region Constructor Methods

		public SynchronisedDictionary()
			: this(new Dictionary<TKey, TValue>())
		{ }

		public SynchronisedDictionary(IDictionary<TKey, TValue> innerDictionary)
		{
			this.innerDictionary = innerDictionary;
		}

		#endregion

		#region IDictionary<TKey,TValue> Members

		public void Add(TKey key, TValue value)
		{
			this.syncLock.AcquireWriterLock(this.lockTimeout);
			try
			{
				this.innerDictionary.Add(key, value);
			}
			finally
			{
				this.syncLock.ReleaseWriterLock();
			}
		}

		public bool ContainsKey(TKey key)
		{
			this.syncLock.AcquireReaderLock(this.lockTimeout);
			try
			{
				return this.ContainsKey(key);
			}
			finally
			{
				this.syncLock.ReleaseReaderLock();
			}
		}

		public ICollection<TKey> Keys
		{
			get {
				this.syncLock.AcquireReaderLock(this.lockTimeout);
				try
				{
					return new List<TKey>(this.innerDictionary.Keys);
				}
				finally
				{
					this.syncLock.ReleaseReaderLock();
				}
			}
		}

		public bool Remove(TKey key)
		{
			this.syncLock.AcquireWriterLock(this.lockTimeout);
			try
			{
				return this.innerDictionary.Remove(key);
			}
			finally
			{
				this.syncLock.ReleaseWriterLock();
			}
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			this.syncLock.AcquireReaderLock(this.lockTimeout);
			try
			{
				return this.innerDictionary.TryGetValue(key, out value);
			}
			finally
			{
				this.syncLock.ReleaseReaderLock();
			}
		}

		public ICollection<TValue> Values
		{
			get {
				this.syncLock.AcquireReaderLock(this.lockTimeout);
				try
				{
					return new List<TValue>(this.innerDictionary.Values);
				}
				finally
				{
					this.syncLock.ReleaseReaderLock();
				}
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				this.syncLock.AcquireReaderLock(this.lockTimeout);
				try
				{
					return this.innerDictionary[key];
				}
				finally
				{
					this.syncLock.ReleaseReaderLock();
				}
			}
			set
			{
				this.syncLock.AcquireWriterLock(this.lockTimeout);
				try
				{
					this.innerDictionary[key] = value;
				}
				finally
				{
					this.syncLock.ReleaseWriterLock();
				}
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			this.syncLock.AcquireWriterLock(this.lockTimeout);
			try
			{
				this.innerDictionary.Add(item);
			}
			finally
			{
				this.syncLock.ReleaseWriterLock();
			}
		}

		public void Clear()
		{
			this.syncLock.AcquireWriterLock(this.lockTimeout);
			try
			{
				this.innerDictionary.Clear();
			}
			finally
			{
				this.syncLock.ReleaseWriterLock();
			}
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			this.syncLock.AcquireReaderLock(this.lockTimeout);
			try
			{
				return this.innerDictionary.Contains(item);
			}
			finally
			{
				this.syncLock.ReleaseReaderLock();
			}
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			this.syncLock.AcquireReaderLock(this.lockTimeout);
			try
			{
				this.innerDictionary.CopyTo(array, arrayIndex);
			}
			finally
			{
				this.syncLock.ReleaseReaderLock();
			}
		}

		public int Count
		{
			get {
				this.syncLock.AcquireReaderLock(this.lockTimeout);
				try
				{
					return this.innerDictionary.Count;
				}
				finally
				{
					this.syncLock.ReleaseReaderLock();
				}
			}
		}

		public bool IsReadOnly
		{
			get { return this.innerDictionary.IsReadOnly; }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			this.syncLock.AcquireWriterLock(this.lockTimeout);
			try
			{
				return this.innerDictionary.Remove(item);
			}
			finally
			{
				this.syncLock.ReleaseWriterLock();
			}
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			this.syncLock.AcquireReaderLock(this.lockTimeout);
			try
			{
				return new List<KeyValuePair<TKey, TValue>>(this.innerDictionary).GetEnumerator();
			}
			finally
			{
				this.syncLock.ReleaseReaderLock();
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			this.syncLock.AcquireReaderLock(this.lockTimeout);
			try
			{
				return ((System.Collections.IEnumerable)new List<KeyValuePair<TKey, TValue>>(this.innerDictionary)).GetEnumerator();
			}
			finally
			{
				this.syncLock.ReleaseReaderLock();
			}
		}

		#endregion

		#region Additional Members

		public ReaderWriterLock SyncLock
		{
			get { return this.syncLock; }
		}

		#endregion

		#region IDeserializationCallback Members

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			this.syncLock = new ReaderWriterLock();
		}

		#endregion
	}
}
