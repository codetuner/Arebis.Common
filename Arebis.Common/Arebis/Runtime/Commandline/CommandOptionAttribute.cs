using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Arebis.Runtime.Commandline
{
	/// <summary>
	/// Maps a field or property to a command-line option.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class CommandOptionAttribute : Attribute
	{
		private string name;
		private string shortName;
		private bool required;
		private object defaultValue;

		/// <summary>
		/// Maps the field or property to a non-mandatory command-line option
		/// </summary>
		public CommandOptionAttribute(string name, object defaultValue)
		{
			this.name = name;
			this.defaultValue = defaultValue;
            this.required = false;
        }

		/// <summary>
		/// Maps the field or property to a non-mandatory command-line option
		/// </summary>
		public CommandOptionAttribute(string name)
		{
			this.name = name;
			this.defaultValue = null;
            this.required = true;
        }

		/// <summary>
		/// The name of the command-line option.
		/// </summary>
		public string Name
		{
			get { return this.name; }
		}

		/// <summary>
		/// Shorthand name of the command-line option.
		/// </summary>
		public string ShortName
		{
			get { return this.shortName; }
            set { this.shortName = value; }
		}

		/// <summary>
		/// The default value of the command-line option.
		/// </summary>
		public object DefaultValue
		{
			get { return this.defaultValue; }
		}

		/// <summary>
		/// Whether the command-line option is required.
		/// </summary>
		public bool IsRequired
		{
			get { return this.required; }
		}

        /// <summary>
        /// Retrieves the attribute applied to the given member.
        /// </summary>
        public static CommandOptionAttribute Get(MemberInfo member)
        {
            return (CommandOptionAttribute)Attribute.GetCustomAttribute(member, typeof(CommandOptionAttribute), true);
        }

        /// <summary>
        /// Retrieves the attribute applied to the given member.
        /// </summary>
        public static CommandOptionValueAttribute[] GetValues(MemberInfo member)
        {
            return (CommandOptionValueAttribute[])Attribute.GetCustomAttributes(member, typeof(CommandOptionValueAttribute), true);
        }

        /// <summary>
        /// Returns the valuetype of the member.
        /// </summary>
        public static Type GetValueType(MemberInfo member)
        {
            if (member is FieldInfo)
            {
                return ((FieldInfo)member).FieldType;
            }
            else if (member is PropertyInfo)
            {
                return ((PropertyInfo)member).PropertyType;
            }
            else
                throw new InvalidOperationException();
        }
    }
}
