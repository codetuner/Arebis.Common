using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Extensions;

namespace Arebis.Utils
{
    /// <summary>
    /// Utility class containing several static utility methods operating on strings.
    /// </summary>
    [Obsolete("Use Arebis.Extensions.StringExtension extensions instead.")]
    public static class StringUtils
	{
        /// <summary>
        /// Shortens the given string to the given length minus the length of the prefix and
        /// the postfix. The returned string is made of prefix + shortened string + postfix.
        /// </summary>
        [Obsolete("Use String.Shorten() extension method instead, see Arebis.Extensions.StringExtension type.")]
        public static string ShortenString(string s, int length, string prefix, string postfix)
        {
            return s.Shorten(length, prefix, postfix);
        }

        /// <summary>
        /// Shortens the given string to the given length if needed. This is done by first 
        /// removing vowels, then, if still too long, trimming the string.
        /// </summary>
        [Obsolete("Use String.Shorten() extension method instead, see Arebis.Extensions.StringExtension type.")]
        public static string ShortenString(string s, int length)
        {
            return s.Shorten(length);
        }

		/// <summary>
		/// Generates an identifier based on the given string. The identifier is
		/// guaranteed to start with a letter or an underscore, and contains only
		/// letters, numbers and underscores.
		/// </summary>
        [Obsolete("Use String.ToIdentifier() extension method instead, see Arebis.Extensions.StringExtension type.")]
        public static string ToIdentifier(string s)
		{
            return s.ToIdentifier();
		}
	}
}
