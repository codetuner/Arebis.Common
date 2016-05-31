// Infrastructure packages.
using System;
using System.Collections;

namespace Arebis.Runtime.Serialization.Formatters.Xml
{
	/// <summary>
	/// Determines whether the specified Object instances are the same instance.
	/// </summary>
	internal class ReferenceComparer : IComparer
	{
		#region Constructors
		public ReferenceComparer()
		{
		}
		#endregion Constructors

		#region IComparer Members
		/// <summary>
		/// Determines whether the specified Object instances are the same instance.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		/// Zero (0) if x is the same instance as y or if both are null references; otherwise, one (1). 
		/// </returns>
		public int Compare(object x, object y)
		{
			int result = Object.ReferenceEquals(x, y) ? 0 : 1;
			return result;
		}
		#endregion IComparer Members
	}
}
