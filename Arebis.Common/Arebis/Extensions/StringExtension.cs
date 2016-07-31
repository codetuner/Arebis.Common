using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Arebis.Extensions
{
    /// <summary>
    /// Provides extension methods to the String class.
    /// </summary>
    /// <remarks>
    /// As Extension Methods will be supported by the C# language, these
    /// methods will be changed into real Extension Methods.
    /// </remarks>
    public static class StringExtension
    {
        /// <summary>
        /// Returns the beginning of the value up to the given marker.
        /// If the marker is not present, returns the entire value.
        /// </summary>
        public static string UpTo(this string value, string marker)
        {
            if (value == null) return null;

            int index = value.IndexOf(marker);
            if (index < 0)
                return value;
            else
                return value.Substring(0, index);
        }

        /// <summary>
        /// Returns an array with all index position where the searchedString appears.
        /// </summary>
        public static int[] AllIndexesOf(this string value, string searchedString, StringComparison comparisonType)
        {
            if (value == null) return null;

            List<int> indexes = new List<int>();
            int pos = 0;
            while (true)
            {
                pos = value.IndexOf(searchedString, pos, comparisonType);
                if (pos == -1) break;
                indexes.Add(pos);
                pos += searchedString.Length;
            }
            return indexes.ToArray();
        }

        /// <summary>
        /// Returns a RegEx to match the given pattern with wildcards * and ?.
        /// </summary>
        public static Regex GetLikeRegex(this string pattern, RegexOptions options)
        {
            pattern = Regex.Escape(pattern);
            pattern = "^" + pattern.Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return new Regex(pattern, options);
        }

        /// <summary>
        /// Whether the given string is a valid Email address.
        /// </summary>
        public static bool IsValidEmailAddress(this string str)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(str);
        }

        /// <summary>
        /// Performs a string pattern matching on a partern with wildcards * and ?.
        /// </summary>
        public static bool Like(this string str, string pattern)
        {
            // Null never matches, but empty string or whitespaces could:
            if (str == null) return false;

            return GetLikeRegex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline).IsMatch(str);
        }

        /// <summary>
        /// Returns a section of the string (similar to Substring() method, but with endindex instead of length argument.
        /// </summary>
        public static string Sectionstring(this string str, int startIndex, int endIndex)
        {
            if (str == null) return null;

            return str.Substring(startIndex, endIndex - startIndex + 1);
        }

        /// <summary>
        /// Translates a PascalCased name into camelCase.
        /// </summary>
        public static string ToCamelCase(this string value)
        {
            if (String.IsNullOrEmpty(value)) return value;

            char[] chars = value.ToCharArray();
            int conversionsDone = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                if ((chars[i] >= 'A') && (chars[i] <= 'Z'))
                {
                    chars[i] = Char.ToLowerInvariant(chars[i]);
                    conversionsDone++;
                }
                else if (chars[i] == '_')
                {
                    continue;
                }
                else
                {
                    if (conversionsDone > 1)
                    {
                        chars[i - 1] = Char.ToUpperInvariant(chars[i - 1]);
                    }
                    break;
                }
            }
            return new String(chars);
        }

        /// <summary>
        /// Capitalizes every first letter of a word.
        /// </summary>
        /// <param name="value">The string to capitalize.</param>
        /// <param name="culture">Culture to use for capitalization. If null, uses current UI culture.</param>
        /// <param name="andLowerNextChars">Whether non-first word characters should be lowered.</param>
        public static string ToCapitalizedWords(this string value, CultureInfo culture = null, bool andLowerNextChars = false)
        {
            if (String.IsNullOrEmpty(value)) return value;

            culture = culture ?? CultureInfo.CurrentUICulture ?? CultureInfo.CurrentCulture ?? CultureInfo.InvariantCulture;

            var chars = value.ToCharArray();
            chars[0] = Char.ToUpper(chars[0], culture);
            for (int i = 1; i < chars.Length; i++)
            {
                if (chars[i-1] == ' ')
                    chars[i] = Char.ToUpper(chars[i], culture);
                else if (chars[i - 1] == '\'')
                    Object.Equals(null, null); // Leave as is
                else if (chars[i - 1] == '-')
                    Object.Equals(null, null); // Leave as is
                else if (andLowerNextChars)
                    chars[i] = Char.ToLower(chars[i], culture);
            }

            return new String(chars);
        }

        /// <summary>
        /// Translates the (multiline) text into an array of lines.
        /// </summary>
        public static string[] ToLines(this string text)
        {
            if (text == null) return new string[0];

            string temp = text;
            temp = temp.Replace("\r\n", "\n");
            temp = temp.Replace('\r', '\n');
            return temp.Split('\n');
        }

        /// <summary>
        /// Truncates the string to the given max length.
        /// </summary>
        public static string Truncate(this string text, int maxLength, bool trimEnd = false)
        {
            if (text == null) return null;

            string result;
            if (text.Length <= maxLength) result = text;
            else result = text.Substring(0, maxLength);

            if (trimEnd) result = result.TrimEnd(' ', '\r', '\n', '\t');
            return result;
        }

        /// <summary>
        /// Translates the string with a switch/case like structure.
        /// First arg is the string to compare to, second arg is the return value
        /// if the string matches the first arg.
        /// If the count of cases is odd, the last value represents a default value,
        /// if the count of cases is even, when no match, null is returned.
        /// </summary>
        public static string CaseTranslate(this string str, StringComparison comparisonType, params string[] cases)
        {
            // Search for a matching case:
            for (int i = 0; i < cases.Length; i += 2)
            {
                if (String.Equals(str, cases[i], comparisonType))
                    return cases[i + 1];
            }

            // If no matching case found, return default value:
            if ((cases.Length % 2) == 1)
                return cases[cases.Length - 1];
            else
                return null;
        }

        /// <summary>
        /// Splits the string into substrings by the given separator string.
        /// </summary>
        public static string[] SplitString(this string str, string separator, int count = -1)
        {
            // Handle special value:
            if (str == null) return null;
            if (String.IsNullOrEmpty(separator)) return new string[] { str };

            var parts = new List<string>();
            var cursor = 0;
            var lastIndex = -1;

            // Split into parts:
            while ((lastIndex = str.IndexOf(separator, cursor)) > -1)
            {
                parts.Add(str.Substring(cursor, lastIndex - cursor));
                cursor = lastIndex + separator.Length;

                if (count >= 0 && parts.Count == (count-1)) break;
            }

            // Append last part:
            parts.Add(str.Substring(cursor));

            // Return result as array (the expected return type of a Split):
            return parts.ToArray();
        }

        /// <summary>
        /// Shortens the given string to the given length minus the length of the prefix and
        /// the postfix. The returned string is made of prefix + shortened string + postfix.
        /// </summary>
        public static string Shorten(this string s, int length, string prefix, string postfix)
        {
            return prefix + Shorten(s, length - prefix.Length - postfix.Length) + postfix;
        }

        /// <summary>
        /// Shortens the given string to the given length if needed. This is done by first 
        /// removing vowels, then, if still too long, trimming the string.
        /// </summary>
        public static string Shorten(this string s, int length)
        {
            if (s == null) return null;

            if (s.Length <= length) return s;

            int toremove = s.Length - length;

            // First try to, remove vowels at the end:
            StringBuilder sb = new StringBuilder(s.Length);
            char[] vowels = new char[] { 'A', 'E', 'I', 'O', 'U', 'a', 'e', 'i', 'o', 'u' };
            int pos = s.Length;
            while (pos > 0)
            {

                pos--;

                if (toremove > 0)
                {
                    // Skip vowels except first one or one just before an underscore:
                    if (Array.Exists(vowels, (Predicate<char>)delegate(char c) { return c == s[pos]; })
                        && (pos > 0)
                        && (s[pos - 1] != '_')
                        )
                    {
                        toremove--;
                        continue;
                    }
                }

                // Append next char (or all next chars if done):
                if (toremove > 0)
                {
                    sb.Insert(0, s[pos]);
                }
                else
                {
                    // Append vowels & non-vowels one name sufficiently shortened:
                    sb.Insert(0, s.Substring(0, pos + 1));
                    pos = 0;
                }
            }

            // If sufficiently reduced, return it:
            if (sb.Length <= length) return sb.ToString();

            // Otherwise, trim last part:
            return sb.ToString().Substring(0, length);
        }

        /// <summary>
        /// Generates an identifier based on the given string. The identifier is
        /// guaranteed to start with a letter or an underscore, and contains only
        /// letters, numbers and underscores.
        /// </summary>
        public static string ToIdentifier(this string s)
        {
            if (s == null) return null;

            StringBuilder identifier = new StringBuilder(s.Length);
            int pos = -1;
            foreach (char c in s.ToCharArray())
            {
                pos++;
                if (((c >= '0') && (c <= '9')))
                {
                    if (pos == 0) identifier.Append("Id");
                    identifier.Append(c);
                }
                else if (((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z')))
                    identifier.Append(c);
                else if (c == ' ')
                    identifier.Append("");
                else
                    identifier.Append('_');
            }
            if (identifier.Length == 0) identifier.Append("BlankIdentifier");
            return identifier.ToString();
        }

        /// <summary>
        /// Returns the requested substring, unless the string is too short, then returns
        /// the best match. If the string is null, returns null.
        /// </summary>
        public static string TrySubstring(this string value, int startIndex, int length)
        {
            if (value == null)
                return value;
            else if (value.Length < (startIndex + length))
                return value.Substring(startIndex);
            else
                return value.Substring(startIndex, length);
        }

        public static string IfNullOrEmpty(this string value, string altValue)
        {
            return (String.IsNullOrEmpty(value)) ? altValue : value;
        }

        public static string IfNullOrWhiteSpace(this string value, string altValue)
        {
            return (String.IsNullOrWhiteSpace(value)) ? altValue : value;
        }
    }
}
