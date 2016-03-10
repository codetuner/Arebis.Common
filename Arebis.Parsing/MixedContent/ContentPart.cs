using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Text;

namespace Arebis.Parsing.MultiContent
{
	/// <summary>
	/// Represents a part of the content of a MixedContentFile.
	/// </summary>
    public class ContentPart
    {
		private MixedContentFile file;
		private object type;
		private int offset;
		private int length;
		private Regex regex;
		private Match match;
		private NameValueCollection data;

		/// <summary>
		/// Constructs a ContentPart matching the whole file content.
		/// </summary>
		public ContentPart(MixedContentFile file, object contentType)
		{
			this.file = file;
			this.type = contentType;
			this.offset = 0;
			this.length = file.Content.Length;
			this.regex = null;
			this.match = null;
			this.data = new NameValueCollection();
		}

		/// <summary>
		/// Constructs a ContentPart.
		/// </summary>
		public ContentPart(MixedContentFile file, object type, int offset, int length, Regex regex, Match match)
		{
			this.file = file;
			this.type = type;
			this.offset = offset;
			this.length = length;
			this.regex = regex;
			this.match = match;

			if (regex == null || match == null)
			{
				this.data = new NameValueCollection();
			}
			else
			{
				this.data = this.DataOfMatch(regex, match);
			}
		}

		/// <summary>
		/// Modifies the content of the current part to have its
		/// source from a different file. Typically used to implement an
		/// #include functionality.
		/// </summary>
		/// <param name="file">The file to 'include'.</param>
		/// <param name="type">The initial type of the substituted section.</param>
		public void Substitute(MixedContentFile file, object type)
		{
			this.file = file;
			this.type = type;
			this.offset = 0;
			this.length = file.Content.Length;
			this.regex = null;
			this.match = null;
			this.data = new NameValueCollection();
		}

		/// <summary>
		/// File of this part.
		/// </summary>
		public MixedContentFile File
		{
			get { return this.file; }
			set { this.file = value; }
		}

		/// <summary>
		/// Type of the part.
		/// </summary>
        public object Type
        {
            get { return this.type; }
			set { this.type = value; }
        }

		/// <summary>
		/// Character position in file at which this part starts.
		/// </summary>
        public int Offset
        {
            get { return this.offset; }
			set { this.offset = value; }
		}

		/// <summary>
		/// Length (number of charaters) of this part.
		/// </summary>
        public int Length
        {
            get { return this.length; }
			set { this.length = value; }
		}

		/// <summary>
		/// Regex used to create this part.
		/// </summary>
		public Regex Regex
		{
			get { return this.regex; }
			set { this.regex = value; }
		}

		/// <summary>
		/// Regex match that created this part.
		/// </summary>
		public Match Match
		{
			get { return this.match; }
			set { this.match = value; }
		}

		/// <summary>
		/// Additional data related to this part.
		/// </summary>
		public NameValueCollection Data
		{
			get { return this.data; }
			set { this.data = value; }
		}

		/// <summary>
		/// Line in file at wchich this part starts.
		/// </summary>
		public int StartLine
		{
			get { return this.File.GetLineNumber(this.offset); }
		}

		/// <summary>
		/// Full text content rerpesented by this part.
		/// </summary>
		public string Content
		{
			get { return this.File.Content.Substring(this.Offset, this.Length); }
		}

		/// <summary>
		/// Builds a NameValueCollection with groupnames/content from a Match.
		/// </summary>
		private NameValueCollection DataOfMatch(Regex regex, Match match)
		{
			NameValueCollection data = new NameValueCollection();
			foreach (string groupname in regex.GetGroupNames())
			{
				foreach (Capture capture in match.Groups[groupname].Captures)
				{
					data.Add(groupname, capture.Value);
				}
			}
			return data;
		}

		/// <summary>
		/// Gives a string representation of the part.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			string content = this.Content;
			content = content.Replace("\r", "\\r");
			content = content.Replace("\n", "\\n");
			content = content.Replace("\t", "\\t");
			sb.Append('\"');
			if (content.Length > 32)
			{
				sb.Append(content.Substring(0, 16));
				sb.Append("…");
				sb.Append(content.Substring(content.Length - 15, 15));
			}
			else
			{
				sb.Append(content);
			}
			sb.Append("\" (");
			sb.Append(Convert.ToString(this.Type));
			sb.Append(")");
			return sb.ToString();
		}
    }
}
