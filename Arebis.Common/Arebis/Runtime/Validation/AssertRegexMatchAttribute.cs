using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Arebis.Runtime.Validation
{
	/// <summary>
	/// Asserts the string value of the property matches the given regular expression (pattern).
	/// </summary>
	public class AssertRegexMatchAttribute : PropertyAssertAttribute
	{
		private Regex regex;

		/// <summary>
		/// Asserts the string value of the property matches the given regular expression (pattern).
		/// </summary>
		public AssertRegexMatchAttribute(string pattern)
			: this(pattern, RegexOptions.None)
		{
		}

		/// <summary>
		/// Asserts the string value of the property matches the given regular expression (pattern).
		/// </summary>
		public AssertRegexMatchAttribute(string pattern, RegexOptions options)
			: this(new Regex(pattern, options))
		{
		}

		/// <summary>
		/// Asserts the string value of the property matches the given regular expression (pattern).
		/// </summary>
		public AssertRegexMatchAttribute(Regex regex)
			: base("String does not match regular expression.")
		{
			this.regex = regex;
		}

		/// <summary>
		/// Returns true if the given property value is valid, false otherwise.
		/// </summary>
		public override bool Validate(object value)
		{
			string strvalue = value as string;
			return ((value == null) || (regex.IsMatch(strvalue)));
		}
	}
}
