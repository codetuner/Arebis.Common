using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
	public static class TypeExtensions
	{
		/// <summary>
		/// Whether the target is if type T.
		/// </summary>
		public static bool IsA<T>(this Type target)
		{
			return (typeof(T).IsAssignableFrom(target));
		}
	}
}
