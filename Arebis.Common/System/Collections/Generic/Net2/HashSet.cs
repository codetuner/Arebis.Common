using System;
using System.Text;

namespace System.Collections.Generic.Net2
{
	/// <summary>
	/// A collection of unique unordered members.
	/// </summary>
	/// <typeparam name="T">Member type.</typeparam>
	[Serializable]
	[Obsolete("Since .NET 3.5, prefer System.Collections.Generic.HashSet, from System.Core.dll.", false)]
	public class HashSet<T> : ICollection<T>
	{
		private Dictionary<T, object> internalList;

		#region Constructor methods

		public HashSet()
			: this(new DefaultEqualityComparer<T>())
		{ }

		public HashSet(IEqualityComparer<T> customComparer)
		{
			this.internalList = new Dictionary<T, object>(customComparer);
		}

		public HashSet(IEnumerable<T> range)
			: this()
		{
			this.AddRange(range);
		}

		public HashSet(IEnumerable<T> range, IEqualityComparer<T> customComparer)
			: this(customComparer)
		{
			this.AddRange(range);
		}

		#endregion Constructor methods

		#region ICollection<T> Members

		public void Add(T item)
		{
			if (!this.internalList.ContainsKey(item))
				this.internalList.Add(item, null);
		}

		public void Clear()
		{
			this.internalList.Clear();
		}

		public bool Contains(T item)
		{
			return this.internalList.ContainsKey(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.internalList.Keys.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return this.internalList.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			return this.internalList.Remove(item);
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return this.internalList.Keys.GetEnumerator();
		}

		#endregion

		#region Additional Members

		public void AddRange(IEnumerable<T> range)
		{
			foreach (T item in range)
				this.Add(item);
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((System.Collections.IEnumerable)this.internalList.Keys).GetEnumerator();
		}

		#endregion

		#region Default Comparer

		[Serializable]
		private class DefaultEqualityComparer<U> : IEqualityComparer<U>
		{
			public bool Equals(U x, U y)
			{
				if (Object.ReferenceEquals(x, y))
					return true;
				else if (Object.ReferenceEquals(x, null))
					return false;
				else
					return x.Equals(y);
			}

			public int GetHashCode(U obj)
			{
				return obj.GetHashCode();
			}
		}

		#endregion
	}
}
