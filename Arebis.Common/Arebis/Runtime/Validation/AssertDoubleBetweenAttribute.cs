using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Runtime.Validation
{
	/// <summary>
	/// Asserts the value of the property is between the given boundaries.
	/// </summary>
	public class AssertDoubleBetweenAttribute : PropertyAssertAttribute
	{
		private double minValue, maxValue;

		/// <summary>
		/// Asserts the value of the property is between the given boundaries.
		/// </summary>
		public AssertDoubleBetweenAttribute(double minValue, double maxValue)
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
			double doublevalue = Convert.ToDouble(value);
			return ((value == null) || ((doublevalue >= minValue) && (doublevalue <= maxValue)));
		}
	}
}
