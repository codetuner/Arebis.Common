using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Arebis.Runtime.Validation
{
	/// <summary>
	/// Object extensions related to validation.
	/// </summary>
	public static class ValidationExtensions
	{
		/// <summary>
		/// Checks validity of the current instance through PropertyAssertAttributes
		/// on public non-indexed properties.
		/// </summary>
		/// <returns>True if the instance is valid.</returns>
		public static bool IsValid(this Object instance)
		{
			// Run over all properties:
			foreach (PropertyInfo prop in instance.GetType().GetProperties())
			{
				// Indexed properties are not supported:
				if (prop.GetIndexParameters().Length > 0) 
					continue;

				// Validate against each PropertyAssertAttribute:
				foreach (PropertyAssertAttribute assertAttr in (PropertyAssertAttribute[])prop.GetCustomAttributes(typeof(PropertyAssertAttribute), true))
				{
					if (assertAttr.Validate(prop.GetValue(instance, null)) == false) 
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Asserts validity of the current instance through PropertyAssertAttributes
		/// on public non-indexed properties. Throws an <see cref="AssertionFailedException" />
		/// on failure.
		/// </summary>
		public static void AssertValid(this Object instance)
		{
			// Run over all properties:
			foreach (PropertyInfo prop in instance.GetType().GetProperties())
			{
				// Indexed properties are not supported:
				if (prop.GetIndexParameters().Length > 0)
					continue;

				// Assert each PropertyAssertAttribute:
				foreach (PropertyAssertAttribute assertAttr in (PropertyAssertAttribute[])prop.GetCustomAttributes(typeof(PropertyAssertAttribute), true))
				{
					assertAttr.Assert(prop.GetValue(instance, null));
				}
			}
		}
	}
}
