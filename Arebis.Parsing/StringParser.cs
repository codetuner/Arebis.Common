using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using Arebis.Extensions;

namespace Arebis.Parsing
{
	/// <summary>
	/// Utility class providing methods to help parsing string based data.
	/// </summary>
	public static class StringParser
	{
		private static Regex rxParseNameValues = new Regex("\\s*(?<name>[^=;]+)=(\"(?<value>[^\"]*)\"\\s*|(?<value>[^;\"]*))(;|$)+", RegexOptions.Singleline);

		/// <summary>
		/// Parses a string containing names and values as in 'Name="John";Age=42'
		/// into a NameValueCollection.
		/// </summary>
		public static NameValueCollection ParseNameValues(string str)
		{
			NameValueCollection result = new NameValueCollection();
			if (str != null)
			{
				foreach (Match match in rxParseNameValues.Matches(str))
				{
					string name = match.Groups["name"].Value;
					string value = match.Groups["value"].Value;
					string quote = match.Groups["quote"].Value;
					result.Add(name, value);
				}
			}
			return result;
		}

		/// <summary>
		/// Splits a string into parts based on a given list of tokens.
		/// </summary>
		/// <param name="str">The string to be parsed.</param>
		/// <param name="tokens">The list of tokens.</param>
		/// <param name="includeTokens">Whether the result should include the tokens.</param>
		/// <param name="includeEmptyStrings">Whether empty strings should be stripped out of the result or not.</param>
		/// <param name="comparisonType">Type of string comparison to perform to lookup the tokens.</param>
		/// <returns>An array of sections of the original string where tokens have been used as delimiters.</returns>
		public static string[] SplitByTokens(string str, string[] tokens, bool includeTokens, bool includeEmptyStrings, StringComparison comparisonType)
		{
			// Retrieve all indexpositions where tokens appear:
			List<int> indexes = new List<int>();
			Dictionary<int, string> tokenPositions = new Dictionary<int, string>();
			foreach (string token in tokens)
			{
				foreach (int index in StringExtension.AllIndexesOf(str, token, comparisonType))
				{
					indexes.Add(index);
					tokenPositions.Add(index, token);
				}
			}
			// Sort the indexpositions:
			indexes.Sort();

			// If no tokens found, simply return string:
			if (indexes.Count == 0)
			{
				return new string[] { str };
			}

			// Build result:
			List<string> splits = new List<string>();
			int pos = 0;

			// Add what preceeds the next token, and add eventually the next token:
			foreach (int index in indexes)
			{
				// What preceeds the token:
				splits.Add(StringExtension.Sectionstring(str, pos, index - 1));
				// Add the token:
				if (includeTokens)
				{
					splits.Add(str.Substring(index, tokenPositions[index].Length));
				}
				// Move position:
				pos = index + tokenPositions[index].Length;
			}

			// Add part after last token:
			splits.Add(str.Substring(pos));

			// Remove empty strings if requested:
			if (!includeEmptyStrings)
			{
				for (int i = splits.Count - 1; i >= 0; i--)
					if (splits[i].Equals(String.Empty)) splits.RemoveAt(i);
			}

			// Translate result to array and return:
			return splits.ToArray();
		}
	}
}
