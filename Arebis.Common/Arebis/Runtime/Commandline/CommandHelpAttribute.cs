using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Arebis.Runtime.Commandline
{
	/// <summary>
	/// Marks a boolean field or property as help requestor member.
	/// </summary>
    [CLSCompliant(false)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public class CommandHelpAttribute : Attribute
	{
		private string[] helpOptions;
		private string description;

        /// <summary>
        /// Marks a boolean field or property as help requestor member.
        /// </summary>
        public CommandHelpAttribute(string[] helpOptions, string description)
		{
			this.helpOptions = helpOptions;
			this.description = description;
		}

        /// <summary>
        /// List of the help optionnames.
        /// </summary>
		public string[] HelpOptions
		{
			get { return this.helpOptions; }
		}

        /// <summary>
        /// Description of the help option.
        /// </summary>
		public string Description
		{
			get { return this.description; }
		}

        /// <summary>
        /// Retrieves the attribute applied to the given member.
        /// </summary>
        public static CommandHelpAttribute Get(MemberInfo member)
        {
            return (CommandHelpAttribute)Attribute.GetCustomAttribute(member, typeof(CommandHelpAttribute), true);
        }
	}
}
