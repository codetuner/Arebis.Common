using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Arebis.Data.Entity
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public sealed class AssociationEndBehaviorAttribute : Attribute
	{
		private static AssociationEndBehaviorAttribute defaultInstance = new AssociationEndBehaviorAttribute(String.Empty);

		public AssociationEndBehaviorAttribute(string endName)
		{
			this.EndName = endName;
		}

		public string EndName { get; private set; }
		public bool Owned { get; set; }

		public static AssociationEndBehaviorAttribute GetAttribute(PropertyInfo property)
		{
			return GetAttribute(property.DeclaringType, property.Name);
		}

		public static AssociationEndBehaviorAttribute GetAttribute(Type type, string endName)
		{
			// Loop over attributes and return matching one:
			foreach (AssociationEndBehaviorAttribute item in type.GetCustomAttributes(typeof(AssociationEndBehaviorAttribute), true))
				if (item.EndName == endName)
					return item;

			// If none found, return default one:
			return defaultInstance;
		}
	}
}
