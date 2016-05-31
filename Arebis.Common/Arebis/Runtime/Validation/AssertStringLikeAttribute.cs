using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Extensions;
using System.Text.RegularExpressions;

namespace Arebis.Runtime.Validation
{
	/// <summary>
	/// Asserts the string value of the property matches the given wildcard pattern.
	/// </summary>
	public class AssertStringLikeAttribute : AssertRegexMatchAttribute
	{

		/// <summary>
		/// Asserts the string value of the property matches the given wildcard pattern.
		/// </summary>
		public AssertStringLikeAttribute(string pattern)
			: base (pattern.GetLikeRegex(RegexOptions.IgnoreCase | RegexOptions.Singleline))
		{ }
	}
}
