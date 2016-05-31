using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Arebis.Runtime.Commandline
{
	/// <summary>
	/// Maps a field or property of type list of string to remaining command-line arguments.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class CommandArgListAttribute : Attribute
	{
		private string name;
		private bool isRequired;

		/// <summary>
		/// Maps a field or property of type list of string to remaining command-line arguments.
		/// </summary>
		/// <param name="name">The name of the argumentlist.</param>
		/// <param name="isRequired">Whether the list should contain at list one member.</param>
		public CommandArgListAttribute(string name, bool isRequired)
		{
			this.name = name;
			this.isRequired = isRequired;
		}

		/// <summary>
		/// The name of the argumentlist.
		/// </summary>
		public string Name
		{
			get { return this.name; }
		}

		/// <summary>
		/// Whether the list should contain at list one member.
		/// </summary>
		public bool IsRequired
		{
			get { return this.isRequired; }
		}

		/// <summary>
		/// Retrieves the attribute applied to the given member.
		/// </summary>
		public static CommandArgListAttribute Get(MemberInfo member)
		{
			return (CommandArgListAttribute)Attribute.GetCustomAttribute(member, typeof(CommandArgListAttribute), true);
		}
	}
}
