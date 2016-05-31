using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Arebis.Reflection
{
	/// <summary>
	/// Extension methods using reflection.
	/// </summary>
	public static class ReflectionExtensions
	{
		/// <summary>
		/// Invokes a public method by reflection.
		/// </summary>
		public static object PublicInvokeMethod(this object instance, string methodName, params object[] args)
		{
			return instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance).Invoke(instance, args);
		}

		/// <summary>
		/// Gets a public property by reflection.
		/// </summary>
		public static object PublicGetProperty(this object instance, string propertyName)
		{
			return instance.GetType().GetProperty(propertyName).GetValue(instance, null);
		}

		/// <summary>
		/// Sets a public property by reflection.
		/// </summary>
		public static void PublicSetProperty(this object instance, string propertyName, object value)
		{
			instance.GetType().GetProperty(propertyName).SetValue(instance, value, null);
		}
	}
}
