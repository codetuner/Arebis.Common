using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace Arebis.Collections.Generic
{
	/// <summary>
	/// An IList wrapper that provides support for list change notification.
	/// </summary>
	[Serializable]
	public class NotifyList<T> : IList<T>, INotifyListChanged
	{
		private IList<T> innerList;

		#region Constructor Methods

		public NotifyList()
			: this(new List<T>())
		{ }

		public NotifyList(IList<T> innerList)
		{
			this.innerList = innerList;
		}

		#endregion

		#region IList<T> Members

		public int IndexOf(T item)
		{
			return innerList.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			this.innerList.Insert(index, item);
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
		}

		public void RemoveAt(int index)
		{
			this.innerList.RemoveAt(index);
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
		}

		public T this[int index]
		{
			get
			{
				return this.innerList[index];
			}
			set
			{
				this.innerList[index] = value;
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			this.innerList.Add(item);
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, this.innerList.Count-1));
		}

		public void Clear()
		{
			this.innerList.Clear();
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		public bool Contains(T item)
		{
			return this.innerList.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.innerList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return this.innerList.Count; }
		}

		public bool IsReadOnly
		{
			get { return this.innerList.IsReadOnly; }
		}

		public bool Remove(T item)
		{
			int index = this.innerList.IndexOf(item);
			if (index > -1)
			{
				this.innerList.RemoveAt(index);
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return this.innerList.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.innerList).GetEnumerator();
		}

		#endregion

		#region INotifyListChanged Members

		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			if (this.ListChanged != null)
				this.ListChanged(this, e);
		}

		public event ListChangedEventHandler ListChanged;

		#endregion

		#region Additional Members

		public void AddRange(IEnumerable<T> range)
		{
			int countbefore = this.innerList.Count;

			foreach (T item in range)
				this.innerList.Add(item);

			this.OnListChanged(
				new ListChangedEventArgs(
					ListChangedType.ItemAdded,
					countbefore - 1,
					this.innerList.Count - 1
				)
			);
		}

		#endregion Additional Members
	}
}
