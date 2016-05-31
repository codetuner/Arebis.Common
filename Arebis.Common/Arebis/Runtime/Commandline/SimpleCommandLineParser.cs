using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace Arebis.Runtime.Commandline
{
	/// <summary>
	/// A simple commandline parser.
	/// </summary>
	public sealed class SimpleCommandLineParser
	{
		private string[] _arguments;
		private IDictionary<string, string> _options;
		private bool _showHelp;

		/// <summary>
		/// Parses the given arguments.
		/// </summary>
		public SimpleCommandLineParser(string[] args)
		{
			ArrayList list = new ArrayList();
			for (int i = 0; i < args.Length; i++)
			{
				char ch = args[i][0];
				if ((ch != '/') && (ch != '-'))
				{
					list.Add(args[i]);
				}
				else
				{
					int index = args[i].IndexOf(':');
					if (index == -1)
					{
						string strA = args[i].Substring(1);
						if ((string.Compare(strA, "help", StringComparison.OrdinalIgnoreCase) == 0) || strA.Equals("?"))
						{
							this._showHelp = true;
						}
						else
						{
							this.Options[strA] = string.Empty;
						}
					}
					else
					{
						this.Options[args[i].Substring(1, index - 1)] = args[i].Substring(index + 1);
					}
				}
			}
			this._arguments = (string[])list.ToArray(typeof(string));
		}

		/// <summary>
		/// The arguments on the commandline.
		/// </summary>
		public string[] Arguments
		{
			get
			{
				return this._arguments;
			}
		}

		/// <summary>
		/// The options on the commandline.
		/// </summary>
		public IDictionary<string, string> Options
		{
			get
			{
				if (this._options == null)
				{
					this._options = new Dictionary<string, string>();
				}
				return this._options;
			}
		}

		/// <summary>
		/// Whether help was requested on the commandline.
		/// </summary>
		public bool ShowHelp
		{
			get
			{
				return this._showHelp;
			}
		}
	}
}
