using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Arebis.Extensions
{
	/// <summary>
	/// Provides extension methods to the Object class.
	/// </summary>
	/// <remarks>
	/// As Extension Methods will be supported by the C# language, these
	/// methods will be changed into real Extension Methods.
	/// </remarks>
	public static class ObjectExtension
	{
		/// <summary>
		/// Whether the given object is inside the given collection or array.
		/// </summary>
		public static bool In(this object obj, IEnumerable collection)
		{
			foreach (object item in collection)
			{
				if (Object.Equals(item, obj)) return true;
			}
			return false;
		}
	}
}
