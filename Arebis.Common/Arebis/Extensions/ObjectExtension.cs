using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

        /// <summary>
        /// Returns a dictionary with all properties of the object and their values.
        /// Returns null if given object is null.
        /// </summary>
        /// <param name="obj">The object to translate into a dictionary.</param>
        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            if (obj == null) return null;

            var result = new Dictionary<string, object>();
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                result[property.Name] = property.GetValue(obj);
            }

            return result;
        }

        /// <summary>
        /// Returns the ToString of the object, or the given defaultString if the object argument is null.
        /// </summary>
        public static string ToStringOr(this object obj, string defaultString)
        {
            if (Object.ReferenceEquals(obj, null))
                return defaultString;
            else
                return obj.ToString();
        }
    }
}
