using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Runtime.Validation
{
	/// <summary>
	/// Asserts the value of the property is not null.
	/// </summary>
	public class AssertNotNullAttribute : PropertyAssertAttribute
	{
		/// <summary>
		/// Asserts the value of the property is not null.
		/// </summary>
		public AssertNotNullAttribute()
			: base("Value should not be null.")
		{}

		/// <summary>
		/// Returns true if the given property value is valid, false otherwise.
		/// </summary>
		public override bool Validate(object value)
		{
			return (ReferenceEquals(value, null) == false);
		}
	}
}
