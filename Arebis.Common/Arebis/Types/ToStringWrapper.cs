using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Types
{
	/// <summary>
	/// A delegate to translate a given object into a string.
	/// </summary>
	public delegate string ToStringDelegate(object obj);

	/// <summary>
	/// Wraps an object and provides it with a custom mechanism to
	/// translate to string.
	/// </summary>
    [Serializable]
	public class ToStringWrapper<T>
	{
		private T value;
		private string toString;
		private ToStringDelegate toStringDelegate;

		/// <summary>
		/// Constructs a wrapper for the given object.
		/// </summary>
		public ToStringWrapper(T value)
		{
			this.value = value;
			this.toStringDelegate = ToStringWrapper<T>.DefaultToStringDelegate;
		}

		/// <summary>
		/// Constructs a wrapper for the given object such that the ToString
		/// returns the given string.
		/// </summary>
		public ToStringWrapper(T value, string toString)
		{
			this.value = value;
			this.toString = toString;
			this.toStringDelegate = this.FixedToStringDelegate;
		}

		/// <summary>
		/// Constructs a wrapper for the given object such that the ToString 
		/// translates to the given ToStringDelegate.
		/// </summary>
		public ToStringWrapper(T value, ToStringDelegate toStringDelegate)
		{
			this.value = value;
			this.toStringDelegate = toStringDelegate;
		}

		/// <summary>
		/// The wrapped object.
		/// </summary>
		public virtual T Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		/// <summary>
		/// Returns a customizable string representation of the wrapped object.
		/// </summary>
		public override string ToString()
		{
			return this.toStringDelegate(this.value);
		}

		private string FixedToStringDelegate(object obj)
		{
			return this.toString;
		}

		private static string DefaultToStringDelegate(object obj)
		{
			return obj.ToString();
		}
	}
}
