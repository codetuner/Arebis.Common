using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
	/// <summary>
	/// A combination of two values usable as key of a dictionary.
	/// Supports null values for any of the key members.
	/// </summary>
	public class Pair<T, U>
	{
		private T first;
		private U second;

		/// <summary>
		/// Creates a new pair based on the given members.
		/// </summary>
		public Pair(T first, U second) {
			this.first = first;
			this.second = second;
		}

		/// <summary>
		/// Returns the first member.
		/// </summary>
		public T First {
			get { return this.first; }
		}

		/// <summary>
		/// Returns the second member.
		/// </summary>
		public U Second
		{
			get { return this.second; }
		}

		/// <summary>
		/// Implements equality based on its members.
		/// </summary>
		public override bool Equals(object obj)
		{
			Pair<T, U> other = obj as Pair<T, U>;
			if (other == null) return false;
			if (!Object.Equals(this.first, other.first)) return false;
			if (!Object.Equals(this.second, other.second)) return false;
			if (!Object.Equals(this.GetType(), other.GetType())) return false;
			return true;
		}

		/// <summary>
		/// Implements hashing based on its members.
		/// </summary>
		public override int GetHashCode()
		{
			int hash = 0;
			if (this.first != null) hash ^= this.first.GetHashCode();
			if (this.second != null) hash ^= this.second.GetHashCode();
			return hash;
		}
	}
}
