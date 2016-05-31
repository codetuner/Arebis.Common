using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Runtime.Validation
{
	/// <summary>
	/// Asserts the value of the property is between the given boundaries.
	/// </summary>
	public class AssertIntegerBetweenAttribute : PropertyAssertAttribute
	{
		private int minValue, maxValue;

		/// <summary>
		/// Asserts the value of the property is between the given boundaries.
		/// </summary>
		public AssertIntegerBetweenAttribute(int minValue, int maxValue)
			: base("Value not between boundaries.")
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
		}

		/// <summary>
		/// Returns true if the given property value is valid, false otherwise.
		/// </summary>
		public override bool Validate(object value)
		{
			int intvalue = Convert.ToInt32(value);
			return ((value == null) || ((intvalue >= minValue) && (intvalue <= maxValue)));
		}
	}
}
