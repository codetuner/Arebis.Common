using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Runtime.Validation
{
	/// <summary>
	/// Asserts the size of the value of the string property is between given boundaries.
	/// </summary>
	public class AssertStringSizeAttribute : PropertyAssertAttribute
	{
		private int minSize, maxSize;
		private bool trimmed;

		/// <summary>
		/// Asserts the size of the value of the string property is between given boundaries.
		/// If trimmed is true, the string value is trimmed before checking its size.
		/// </summary>
		public AssertStringSizeAttribute(int minSize, int maxSize, bool trimmed)
			: base ("Invalid string size.")
		{
			this.minSize = minSize;
			this.maxSize = maxSize;
			this.trimmed = trimmed;
		}

		/// <summary>
		/// Returns true if the given property value is valid, false otherwise.
		/// </summary>
		public override bool Validate(object value)
		{
			if (value == null) return true;
			string strvalue = value as string;
			if (trimmed) strvalue = strvalue.Trim();
			return ((strvalue.Length >= minSize) && (strvalue.Length <= maxSize));
		}
	}
}
