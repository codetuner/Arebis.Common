using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Arebis.Runtime.Commandline
{
	/// <summary>
	/// Provides help information for command-line arguments and options.
    /// When decorating the options class, provides help information about
    /// the application.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
	public class CommandInfoAttribute : Attribute
	{
		private string description;

		/// <summary>
		/// Provides help information for command-line arguments and options.
		/// </summary>
		public CommandInfoAttribute(string description)
		{
			this.description = description;
		}

		/// <summary>
		/// Description of the command-line argument or option.
		/// </summary>
		public string Description
		{
			get { return this.description; }
		}

        /// <summary>
        /// Retrieves the attribute applied to the given member.
        /// </summary>
        public static CommandInfoAttribute Get(MemberInfo member)
        {
            return (CommandInfoAttribute)Attribute.GetCustomAttribute(member, typeof(CommandInfoAttribute), true);
        }

        /// <summary>
        /// Retrieves the attribute applied to the given type.
        /// </summary>
        public static CommandInfoAttribute Get(Type optionClass)
        {
            return (CommandInfoAttribute)Attribute.GetCustomAttribute(optionClass, typeof(CommandInfoAttribute), true);
        }
    }
}
