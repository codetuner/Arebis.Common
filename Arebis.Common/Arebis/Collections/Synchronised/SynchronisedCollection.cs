using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;

namespace Arebis.Collections.Synchronised
{
	/// <summary>
	/// Wrapper for synchronised access to an inner collection.
	/// </summary>
	[Serializable]
	public class SynchronisedCollection<T> : ICollection<T>, IDeserializationCallback
	{
		private ICollection<T> innerCollection;
		[NonSerialized] private ReaderWriterLock syncLock = new ReaderWriterLock();
		private volatile int lockTimeout = 10000;

		#region Constructor Methods

		public SynchronisedCollection()
			: this(new List<T>())
		{ }

		public SynchronisedCollection(ICollection<T> innerCollection)
		{
			this.innerCollection = innerCollection;
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			this.syncLock.AcquireWriterLock(this.lockTimeout);
			try
			{
				this.innerCollection.Add(item);
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
				this.innerCollection.Clear();
			}
			finally
			{
				this.syncLock.ReleaseWriterLock();
			}
		}

		public bool Contains(T item)
		{
			this.syncLock.AcquireReaderLock(this.lockTimeout);
			try
			{
				return this.innerCollection.Contains(item);
			}
			finally
			{
				this.syncLock.ReleaseReaderLock();
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.syncLock.AcquireReaderLock(this.lockTimeout);
			try
			{
				this.CopyTo(array, arrayIndex);
			}
			finally
			{
				this.syncLock.ReleaseReaderLock();
			}
		}

		public int Count
		{
			get
			{
				this.syncLock.AcquireReaderLock(this.lockTimeout);
				try
				{
					return this.innerCollection.Count;
				}
				finally
				{
					this.syncLock.ReleaseReaderLock();
				}
			}
		}

		public bool IsReadOnly
		{
			get { return this.innerCollection.IsReadOnly; }
		}

		public bool Remove(T item)
		{
			this.syncLock.AcquireWriterLock(this.lockTimeout);
			try
			{
				return this.innerCollection.Remove(item);
			}
			finally
			{
				this.syncLock.ReleaseWriterLock();
			}
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			this.syncLock.AcquireReaderLock(this.lockTimeout);
			try
			{
				return (new List<T>(this.innerCollection)).GetEnumerator();
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
				return ((System.Collections.IEnumerable)new List<T>(this.innerCollection)).GetEnumerator();
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

		public void AddRange(ICollection<T> items)
		{
			this.syncLock.AcquireWriterLock(this.lockTimeout);
			try
			{
				foreach (T item in items)
					this.innerCollection.Add(item);
			}
			finally
			{
				this.syncLock.ReleaseWriterLock();
			}
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
