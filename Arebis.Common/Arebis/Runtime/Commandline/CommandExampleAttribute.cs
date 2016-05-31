using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Runtime.Commandline
{
	/// <summary>
	/// Provides a sample of the commandline. The given sample lists only the arguments
	/// and options, excluding the command iteself.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public class CommandExampleAttribute : Attribute
	{
		private string sample;
		private string description;

		/// <summary>
		/// Provides a sample of the commandline. The given sample lists only the arguments
		/// and options, excluding the command iteself.
		/// </summary>
		public CommandExampleAttribute(string sample)
		{
			this.sample = sample;
		}

		/// <summary>
		/// Provides a sample of the commandline. The given sample lists only the arguments
		/// and options, excluding the command iteself.
		/// </summary>
		public CommandExampleAttribute(string sample, string description)
		{
			this.sample = sample;
			this.description = description;
		}

		/// <summary>
		/// The given commandline arguments sample.
		/// </summary>
		public string Sample
		{
			get { return this.sample; }
		}

		/// <summary>
		/// Description of the sample.
		/// </summary>
		public string Description
		{
			get { return this.description; }
		}

		/// <summary>
		/// Retrieves the attribute applied to the given type.
		/// </summary>
		public static CommandExampleAttribute[] GetValues(Type type)
		{
			return (CommandExampleAttribute[])type.GetCustomAttributes(typeof(CommandExampleAttribute), true);
		}
	}
}
