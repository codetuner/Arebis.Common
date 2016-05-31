using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;

namespace Arebis.Collections.Synchronised
{
	/// <summary>
	/// Wrapper for synchronised access to an inner list.
	/// </summary>
	[Serializable]
	public class SynchronisedList<T> : IList<T>, IDeserializationCallback
	{
		private IList<T> innerList;
		[NonSerialized] private ReaderWriterLock syncLock = new ReaderWriterLock();
		private volatile int lockTimeout = 10000;

		#region Constructor Methods

		public SynchronisedList()
			: this(new List<T>())
		{ }

		public SynchronisedList(IList<T> innerList)
		{
			this.innerList = innerList;
		}

		#endregion

		#region IList<T> Members

		public int IndexOf(T item)
		{
			this.syncLock.AcquireReaderLock(this.lockTimeout);
			try
			{
				return this.innerList.IndexOf(item);
			}
			finally
			{
				this.syncLock.ReleaseReaderLock();
			}
		}

		public void Insert(int index, T item)
		{
			this.syncLock.AcquireWriterLock(this.lockTimeout);
			try
			{
				this.innerList.Insert(index, item);
			}
			finally
			{
				this.syncLock.ReleaseWriterLock();
			}
		}

		public void RemoveAt(int index)
		{
			this.syncLock.AcquireWriterLock(this.lockTimeout);
			try
			{
				this.innerList.RemoveAt(index);
			}
			finally
			{
				this.syncLock.ReleaseWriterLock();
			}
		}

		public T this[int index]
		{
			get
			{
				this.syncLock.AcquireReaderLock(this.lockTimeout);
				try
				{
					return this.innerList[index];
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
					this.innerList[index] = value;
				}
				finally
				{
					this.syncLock.ReleaseWriterLock();
				}
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			this.syncLock.AcquireWriterLock(this.lockTimeout);
			try
			{
				this.innerList.Add(item);
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
				this.innerList.Clear();
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
				return this.innerList.Contains(item);
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
					return this.innerList.Count;
				}
				finally
				{
					this.syncLock.ReleaseReaderLock();
				}
			}
		}

		public bool IsReadOnly
		{
			get { return this.innerList.IsReadOnly; }
		}

		public bool Remove(T item)
		{
			this.syncLock.AcquireWriterLock(this.lockTimeout);
			try
			{
				return this.innerList.Remove(item);
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
				return (new List<T>(this.innerList)).GetEnumerator();
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
				return ((System.Collections.IEnumerable)new List<T>(this.innerList)).GetEnumerator();
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
					this.innerList.Add(item);
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
