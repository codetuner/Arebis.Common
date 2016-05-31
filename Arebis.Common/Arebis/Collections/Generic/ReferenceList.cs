using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Collections.Generic
{
	/// <summary>
	/// IList implementation for objects to be compared by reference
	/// instead of by equality.
	/// </summary>
	/// <remarks>
	/// This implementation can provide a significant performance gain
	/// compared to the standard .NET list for elements with an
	/// overriden Equals implementation.
	/// </remarks>
	public class ReferenceList<T> : IList<T> where T : class
	{
		private int count = 0;
		private int capacity = 8;
		private int increment = 8;
		private T[] values;

		/// <summary>
		/// Constructs a new, empty list.
		/// </summary>
		public ReferenceList() : this(8)
		{
		}

		/// <summary>
		/// Constructs a new, empty list.
		/// </summary>
		/// <param name="initialCapacity">Initial capacity of the list.</param>
		public ReferenceList(int initialCapacity)
		{
			this.capacity = initialCapacity;
			this.values = new T[this.capacity];
		}

		#region IList<T> Members

		/// <summary>
		/// Returns the index of the given element.
		/// </summary>
		public int IndexOf(T item)
		{
			for (int i = 0; i < this.count; i++)
				if (Object.ReferenceEquals(this.values[i], item)) 
					return i;
			return -1;
		}

		/// <summary>
		/// Inserts the given item at the given index position.
		/// </summary>
		public void Insert(int index, T item)
		{
			this.EnsureCapacity(this.count + 1);
			for (int i = this.count; i >= index; i--)
				this.values[i + 1] = this.values[i];
			this.values[index] = item;
			this.count++;
		}

		/// <summary>
		/// Removes the item at the given location.
		/// </summary>
		public void RemoveAt(int index)
		{
			for (int i = (this.count-2); i > index; i--)
				this.values[i] = this.values[i+1];
			this.values[this.count - 1] = null;
			this.count--;
		}

		/// <summary>
		/// An item of the list.
		/// </summary>
		public T this[int index]
		{
			get
			{
				if ((index >= this.count) || (index < 0))
					throw new IndexOutOfRangeException();
				return this.values[index];
			}
			set
			{
				if (index < 0)
					throw new IndexOutOfRangeException();
				if (index >= this.count)
					this.EnsureCapacity(index + 1);
				this.values[index] = value;
				if (index >= this.count)
					this.count = index + 1;
			}
		}

		#endregion

		#region ICollection<T> Members

		/// <summary>
		/// Adds an item to the list.
		/// </summary>
		public void Add(T item)
		{
			this.EnsureCapacity(this.count + 1);
			this.values[this.count] = item;
			this.count++;
		}

		/// <summary>
		/// Clears the list.
		/// </summary>
		public void Clear()
		{
			this.count = 0;
			this.capacity = 8;
			this.values = new T[this.capacity];
		}

		/// <summary>
		/// Whether the list contains the given element.
		/// </summary>
		public bool Contains(T item)
		{
			for (int i = 0; i < this.count; i++)
				if (Object.ReferenceEquals(this.values[i], item)) 
					return true;
			return false;
		}

		/// <summary>
		/// Copies the current list to the given array.
		/// </summary>
		/// <param name="array">Array to copy to.</param>
		/// <param name="arrayIndex">First array index to fill.</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(this.values, 0, array, arrayIndex, this.count);
		}

		/// <summary>
		/// The item count of this list.
		/// </summary>
		public int Count
		{
			get { return this.count; }
		}

		/// <summary>
		/// Whether this list is readonly.
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Removes the (first occurence) of the given item.
		/// </summary>
		/// <returns>True if the item was present and removed, false if the item was not found.</returns>
		public bool Remove(T item)
		{
			int index = this.IndexOf(item);
			if (index >= 0)
			{
				this.RemoveAt(index);
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region IEnumerable<T> Members

		/// <summary>
		/// Returns an Enumerator.
		/// </summary>
		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < this.count; i++)
				yield return this.values[i];
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < this.count; i++)
				yield return this.values[i];
		}

		#endregion

		#region Private implementation

		private void EnsureCapacity(int newCapacity)
		{
			if (this.capacity < newCapacity)
			{
				T[] newValues = new T[newCapacity + this.increment];
				Array.Copy(this.values, newValues, this.count);
				this.values = newValues;
				this.capacity = newCapacity;
			}
		}

		#endregion
	}
}
