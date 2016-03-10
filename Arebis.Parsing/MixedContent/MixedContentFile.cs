using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace Arebis.Parsing.MultiContent
{
	/// <summary>
	/// Represents a file with mixed content.
	/// </summary>
	public class MixedContentFile
	{
		private string filename;
		private string content;
		private int[] lineStartPositions;
		private List<ContentPart> parts = new List<ContentPart>();

		/// <summary>
		/// Constructs a MixedContentFile for the given file having given conetent.
		/// </summary>
		public MixedContentFile(string filename, string content, object initialContentType)
		{
			// Set properties:
			this.filename = filename;
			this.content = content;
			this.lineStartPositions = GetLineStartPositions(this.content);

			// Seed the parts list:
			this.parts.Add(new ContentPart(this, initialContentType));
		}

		/// <summary>
		/// Name of the file.
		/// </summary>
		public string Filename
		{
			get { return this.filename; }
		}

		/// <summary>
		/// Content of the file.
		/// </summary>
		public string Content
		{
			get { return this.content; }
		}

		/// <summary>
		/// Number of lines of the content.
		/// </summary>
		public int LineCount
		{
			get { return this.lineStartPositions.Length; }
		}

		/// <summary>
		/// List of parts the file is made of.
		/// </summary>
		public List<ContentPart> Parts
		{
			get { return this.parts; }
		}

		/// <summary>
		/// Retrieved the parts of given type.
		/// </summary>
		public IEnumerable<ContentPart> FindPartsOfType(object type)
		{
			foreach (ContentPart part in this.Parts)
			{
				if (Object.Equals(part.Type, type)) yield return part;
			}
		}

		/// <summary>
		/// Returns the linenumber of the given character position in the file.
		/// </summary>
		public int GetLineNumber(int position)
		{
			for (int i = 0; i < this.lineStartPositions.Length; i++)
			{
				if (this.lineStartPositions[i] > position) return i;
			}
			return this.lineStartPositions.Length;
		}

		/// <summary>
		/// Applies a parser regular expression to identifiy types of content.
		/// </summary>
		/// <param name="typeToSearch">The content type to search in.</param>
		/// <param name="typeToSearchFor">The content type of the parts matching the given regular expression.</param>
		/// <param name="expressionToSearchFor">The regular expression to use to match content parts.</param>
		/// <returns>True if at least one part was identified, otherwise false.</returns>
		public bool ApplyParserRegex(object typeToSearch, object typeToSearchFor, Regex expressionToSearchFor)
		{
			if (expressionToSearchFor == null) return false;

			bool matchFound = false;
			List<ContentPart> result = new List<ContentPart>();
			foreach (ContentPart section in this.parts)
			{
				if (Object.Equals(section.Type, typeToSearch))
				{
					int pos = 0;
					foreach (Match match in expressionToSearchFor.Matches(section.Content))
					{
						if (pos < match.Index)
						{
							result.Add(new ContentPart(section.File, section.Type, section.Offset + pos, match.Index - pos, null, null));
						}
						result.Add(new ContentPart(section.File, typeToSearchFor, section.Offset + match.Index, match.Length, expressionToSearchFor, match));
						pos = match.Index + match.Length;
						matchFound = true;
					}
					if (pos < section.Length)
					{
						result.Add(new ContentPart(section.File, section.Type, section.Offset + pos, section.Length - pos, null, null));
					}
				}
				else
				{
					result.Add(section);
				}
			}

			// Perform generation substitution:
			this.parts = result;

			// Return whether matches were found:
			return matchFound;
		}

		/// <summary>
		/// Extracts a group in a part to a new part.
		/// </summary>
		/// <param name="typeToPromote">Type to be searched for extractable groups.</param>
		/// <param name="groupName">Name of group to extract from.</param>
		/// <param name="typeToPromoteTo">Type of the new extraced part.</param>
		/// <remarks>
		/// This method replaces parts by group captures in the parts. It is usefull to
		/// strip out delimiters and other unused content.
		/// </remarks>
		public void ExtractPartsGroup(object typeToPromote, string groupName, object typeToPromoteTo)
		{
			List<ContentPart> result = new List<ContentPart>();
			foreach (ContentPart section in this.parts)
			{
				if ((Object.Equals(section.Type, typeToPromote) && (section.Match != null)))
				{
					foreach (Capture item in section.Match.Groups[groupName].Captures)
					{
						result.Add(new ContentPart(section.File, typeToPromoteTo, section.Offset + item.Index - section.Match.Index, item.Length, null, null));
					}
				}
				else
				{
					result.Add(section);
				}
			}

			// Perform generation substitution:
			this.parts = result;
		}

		private static int[] GetLineStartPositions(string text)
		{
			List<int> result = new List<int>();
			foreach (Match match in new Regex(".*(\\r\\n?|.$)").Matches(text))
			{
				result.Add(match.Index);
			}
			return result.ToArray();
		}
	}
}
