using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Arebis.Runtime.Commandline
{
	/// <summary>
	/// Maps a field or property to a command-line argument.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class CommandArgAttribute : Attribute
	{
		private int order;
        private string name;
		private bool required;

        /// <summary>
        /// Maps a field or property to a command-line argument.
        /// </summary>
        [Obsolete("Use constructor providing name.")]
        public CommandArgAttribute(int order, bool required)
        {
            this.order = order;
            this.name = "?";
            this.required = required;
        }

        /// <summary>
        /// Maps a field or property to a command-line argument.
        /// </summary>
        public CommandArgAttribute(int order, string name, bool required)
        {
            this.order = order;
            this.name = name;
            this.required = required;
        }

        /// <summary>
		/// Order of the argument.
		/// </summary>
		public int Order
		{
			get { return this.order; }
		}

        /// <summary>
        /// The name of the argument (as shown in help).
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

		/// <summary>
		/// Whether the argument is required.
		/// </summary>
		public bool IsRequired
		{
			get { return this.required; }
		}

        /// <summary>
        /// Retrieves the attribute applied to the given member.
        /// </summary>
        public static CommandArgAttribute Get(MemberInfo member)
        {
            return (CommandArgAttribute)Attribute.GetCustomAttribute(member, typeof(CommandArgAttribute), true);
        }
	}
}
