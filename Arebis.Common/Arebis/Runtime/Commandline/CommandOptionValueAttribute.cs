using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Runtime.Commandline
{
	/// <summary>
	/// Declares a value for command-line options with limited allowed value list.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=true)]
	public class CommandOptionValueAttribute : Attribute
	{
		private object value;
		private string description;
		private string uservalue;

		/// <summary>
		/// Declares a value for command-line options with limited allowed value list.
		/// </summary>
		public CommandOptionValueAttribute(object value, string description)
		{
			this.uservalue = Convert.ToString(value);
			this.value = value;
			this.description = description;
		}

		/// <summary>
		/// Declares a value for command-line options with limited allowed value list.
		/// </summary>
		public CommandOptionValueAttribute(object value, string uservalue, string description)
		{
			this.uservalue = uservalue;
			this.value = value;
			this.description = description;
		}

		/// <summary>
		/// Value of the command-line option value, as entered on the command-line (by the user).
		/// </summary>
		public string UserValue
		{
			get { return this.uservalue; }
		}

		/// <summary>
		/// Value of the command-line option value, as used in code.
		/// </summary>
		public object Value
		{
			get { return this.value; }
		}

		/// <summary>
		/// Description of the command-line option value.
		/// </summary>
		public object Description
		{
			get { return this.description; }
		}
	}
}
