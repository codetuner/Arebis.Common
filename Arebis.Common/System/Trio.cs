using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
	/// <summary>
	/// A combination of three values usable as key of a dictionary.
	/// Supports null values for any of the key members.
	/// </summary>
	public class Trio<T, U, V>
	{
		private T first;
		private U second;
		private V third;

		/// <summary>
		/// Creates a new trio based on the given members.
		/// </summary>
		public Trio(T first, U second, V third)
		{
			this.first = first;
			this.second = second;
			this.third = third;
		}

		/// <summary>
		/// Returns the first member.
		/// </summary>
		public T First
		{
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
		/// Returns the third member.
		/// </summary>
		public V Third
		{
			get { return this.third;}
		}

		/// <summary>
		/// Implements equality based on its members.
		/// </summary>
		public override bool Equals(object obj)
		{
			Trio<Object, Object, Object> other = obj as Trio<Object, Object, Object>;
			if (other == null) return false;
			if (!Object.Equals(this.first, other.first)) return false;
			if (!Object.Equals(this.second, other.second)) return false;
			if (!Object.Equals(this.third, other.third)) return false;
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
			if (this.third != null) hash ^= this.third.GetHashCode();
			return hash;
		}
	}
}
