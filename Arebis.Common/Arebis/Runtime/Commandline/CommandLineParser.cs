using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Arebis.Runtime.Commandline
{
    /// <summary>
    /// Command-line parser using CommandArg, CommandOption,... attributes to 
    /// identify the structure of the command-line.
    /// </summary>
    public class CommandLineParser
    {
        /// <summary>
        /// Marks an option.
        /// Common values are "/" or "-".
        /// </summary>
        private string optionMarker = "/";

        /// <summary>
        /// Separated the option from it's value.
        /// Common values are " " (blanks), ":" or "=".
        /// </summary>
        private string valueSeparator = " ";

        /// <summary>
        /// All possible options to tell help is requested.
        /// </summary>
        private string[] helpOptions = { "?", "h", "help" };

		private bool isHelpRequested = false;

		private bool isMissingArguments = false;

        /// <summary>
        /// Whether help information should be shown.
        /// </summary>
        public bool IsHelpRequested
        {
            get { return this.isHelpRequested; }
        }

		/// <summary>
		/// Whether required arguments are missing.
		/// </summary>
		public bool IsMissingArguments
		{
			get { return this.isMissingArguments; }
		}

        /// <summary>
        /// Parses the environments commandline.
        /// The commandlineinfo type must have fields or properties
        /// decorated with CommandArg, CommandOption,... attributes.
        /// </summary>
        /// <returns>Return the passed commandlineinfo object.</returns>
        public object Parse(object commandlineinfo)
        {
            return this.Parse(commandlineinfo, Environment.CommandLine);
        }

        /// <summary>
        /// Parses the given commandline (in which the first item should
        /// be the executable).
        /// The commandlineinfo type must have fields or properties
        /// decorated with CommandArg, CommandOption,... attributes.
        /// </summary>
        /// <returns>Return the passed commandlineinfo object.</returns>
        [Arebis.Source.CodeToDo("check that all mandatory args and options have been set.")]
        public object Parse(object commandlineinfo, string commandline)
        {
            // Retrieve tokens:
            Queue<string> tokens = new Queue<string>(Tokenize(commandline));

            // Dequeue first argument as it is the executable:
            tokens.Dequeue();

            // Retrieve CommandLineInfo Handler:
            CommandLineInfoHandler handler = new CommandLineInfoHandler(commandlineinfo);

            // Handle tokens:
            int argnumber = 0;
            while (tokens.Count > 0)
            {
                string token = tokens.Dequeue();

                if (token.StartsWith(optionMarker))
                {
                    // Handle option:

                    string option = token.Substring(optionMarker.Length);
                    if (handler.HelpOptions.Contains(option))
                    {
                        // If option is helpoption, mark helpRequested:
                        this.isHelpRequested = true;
                        this.SetOption(commandlineinfo, handler.HelpHandler, true);
                    }
                    else
                    {
                        try
                        {
                            // For other option, first retrieve option name:
                            string optionname;
                            if (this.valueSeparator == " ")
                            {
                                optionname = option;
                            }
                            else if (option.Contains(this.optionMarker))
                            {
                                optionname = option.Substring(0, option.IndexOf(this.optionMarker));
                            }
                            else
                            {
                                optionname = option;
                            }

                            // Retrieve option handler for optionname:
                            MemberInfo opthandler = handler.OptionHandlers[optionname];

                            if (this.IsBooleanOption(opthandler))
                            {
                                // If boolean option, set option to true, as option is set:
                                this.SetOption(commandlineinfo, opthandler, true);
                            }
                            else
                            {
                                // Otherwise retrieve options value:
                                string optionvalue;
                                if (this.valueSeparator == " ")
                                {
                                    if (tokens.Count > 0) optionvalue = tokens.Dequeue();
                                    else optionvalue = null;
                                }
                                else if (option.Contains(this.optionMarker))
                                {
                                    optionvalue = option.Substring(option.IndexOf(this.optionMarker) + this.optionMarker.Length);
                                }
                                else
                                {
                                    optionvalue = null;
                                }

                                // And set option value:
                                this.SetOption(commandlineinfo, opthandler, optionvalue, handler.OptionValues[optionname]);
                            }
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new InvalidOption();
                        }
                    }
                }
                else
                {
                    // Handle argument:

                    if (handler.ArgumentHandlers.Count > argnumber)
                    {
                        this.SetArgument(commandlineinfo, handler.ArgumentHandlers[argnumber], token);
                    }
                    else if (handler.ArgumentListHandler != null)
                    {
                        this.AppendArgument(commandlineinfo, handler.ArgumentListHandler, token);
                    }
                    else
                    {
                        throw new TooManyArgumentsException();
                    }
                    argnumber++;
				}
            }

            // Validate commandline settings:
            if (!this.IsHelpRequested)
            {
                // TODO: check that all mandatory args and options have been set
            }
			if (argnumber < handler.NumberOfRequiredArgs)
				this.isMissingArguments = true;

            // Return the passed commandlineinfo object:
            return commandlineinfo;
        }

        /// <summary>
        /// Tokenizes the given commandline. The passed commandline would
        /// typically be retrieved with Environment.CommandLine (where the
        /// first token is the running executable).
        /// </summary>
        public String[] Tokenize(string commandline)
        {
            List<string> tokens = new List<string>();
            StringBuilder token = null;
            bool quoted = false;

            int chars = commandline.Length;
            string commandlineext = commandline + " ";
            for (int i = 0; i < chars; i++)
            {
                char c = commandline[i];
                switch (c)
                {
                    case '"':
                        if (token == null)
                        {
                            quoted = true;
                            token = new StringBuilder();
                        }
                        else // if (token != null)
                        {
                            if (commandlineext[i + 1] == '"')
                            {
                                // Double quote in token -> single quote:
                                token.Append(c);
                                // Skip next char:
                                i++;
                            }
                            else
                            {
                                tokens.Add(token.ToString());
                                token = null;
                                quoted = false;
                            }
                        }
                        break;
                    case ' ':
                        if (token == null)
                        {
                            // Simply skip
                        }
                        else // if (token != null)
                        {
                            if (quoted == true)
                            {
                                // If quoted: add space:
                                token.Append(c);
                            }
                            else
                            {
                                // If unquoted: token ends:
                                tokens.Add(token.ToString());
                                token = null;
                            }
                        }
                        break;
                    default:
                        if (token == null)
                        {
                            // Start new token:
                            token = new StringBuilder();
                            token.Append(c);
                        }
                        else // if (token != null)
                        {
                            // Add char to token:
                            token.Append(c);
                        }
                        break;
                }
            }

            // Add last token:
            if (token != null)
            {
                tokens.Add(token.ToString());
            }

            return tokens.ToArray();
        }

        private void AppendArgument(object commandlineinfo, MemberInfo argListMember, string arg)
        {
            if (argListMember is FieldInfo)
            {
                FieldInfo member = (FieldInfo)argListMember;

                // Retrieve argument list:
                IList<string> argumentList = (IList<string>)member.GetValue(commandlineinfo);

                // Append argument to list:
                argumentList.Add(arg);
            }
            else // Assume PropertyInfo
            {
                PropertyInfo member = (PropertyInfo)argListMember;

                // Retrieve argument list:
                IList<string> argumentList = (IList<string>)member.GetValue(commandlineinfo, null);

                // Append argument to list:
                argumentList.Add(arg);
            }
        }

        private void SetArgument(object commandlineinfo, MemberInfo argMember, string arg)
        {
            if (argMember is FieldInfo)
            {
                FieldInfo member = (FieldInfo)argMember;
                member.SetValue(commandlineinfo, arg);
            }
            else // Assume PropertyInfo
            {
                PropertyInfo member = (PropertyInfo)argMember;
                member.SetValue(commandlineinfo, arg, null);
            }
        }

        private void SetOption(object commandlineinfo, MemberInfo optMember, string stringValue, Dictionary<string, object> allowedValues)
        {
            // Retrieve value based on stringValue:
            object value;
            if (allowedValues.Count == 0)
            {
                value = stringValue;
            }
            else
            {
                try
                {
                    value = allowedValues[stringValue];
                }
                catch (KeyNotFoundException)
                {
                    throw new InvalidOptionValue();
                }
            }

            // Set value:
            if (optMember is FieldInfo)
            {
                FieldInfo member = (FieldInfo)optMember;
                member.SetValue(commandlineinfo, value);
            }
            else // Assume PropertyInfo
            {
                PropertyInfo member = (PropertyInfo)optMember;
                member.SetValue(commandlineinfo, value, null);
            }
        }

        private void SetOption(object commandlineinfo, MemberInfo optMember, bool value)
        {
            // Set value:
            if (optMember is FieldInfo)
            {
                FieldInfo member = (FieldInfo)optMember;
                member.SetValue(commandlineinfo, value);
            }
            else // Assume PropertyInfo
            {
                PropertyInfo member = (PropertyInfo)optMember;
                member.SetValue(commandlineinfo, value, null);
            }
        }

        private bool IsBooleanOption(MemberInfo optMember)
        {
            if (optMember is FieldInfo)
            {
                return (((FieldInfo)optMember).FieldType == typeof(bool));
            }
            else // Assume PropertyInfo
            {
                return (((PropertyInfo)optMember).PropertyType == typeof(bool));
            }
        }

        /// <summary>
        /// Generate the parameter section of commandline help
        /// based on the given commandlineinfo type.
        /// The commandlineinfo type must have fields or properties
        /// decorated with CommandArg, CommandOption,... attributes.
        /// </summary>
        public string GenerateHelp(Type commandlineinfotype)
        {
            StringBuilder sb = new StringBuilder();

            Assembly entryAssembly = Assembly.GetEntryAssembly();
            MemberInfo help = null;
            SortedList<int, MemberInfo> args = new SortedList<int, MemberInfo>();
            MemberInfo argList = null;
            List<MemberInfo> options = new List<MemberInfo>();

            // Retrieve members:
            foreach (MemberInfo member in commandlineinfotype.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty))
            {
                if (CommandArgAttribute.Get(member) != null)
                    args.Add(CommandArgAttribute.Get(member).Order, member);
                else if (CommandArgListAttribute.Get(member) != null)
                    argList = member;
                else if (CommandOptionAttribute.Get(member) != null)
                    options.Add(member);
                else if (CommandHelpAttribute.Get(member) != null)
                    help = member;
            }

            // Write application title:
            sb.AppendLine(((AssemblyTitleAttribute)entryAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0]).Title);
            sb.AppendLine();

            // Write application description:
            if (commandlineinfotype.GetCustomAttributes(typeof(CommandInfoAttribute), false).Length > 0)
            {
                // Based on CommandInfoAttribute if available:
                sb.AppendLine(((CommandInfoAttribute)commandlineinfotype.GetCustomAttributes(typeof(CommandInfoAttribute), false)[0]).Description);
                sb.AppendLine();
            }
            else
            { 
                // Else based on AssemblyDescriptionAttribute:
                var descattr = ((AssemblyDescriptionAttribute)entryAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]);
                if ((descattr != null) && (!String.IsNullOrWhiteSpace(descattr.Description)))
                {
                    sb.AppendLine(descattr.Description);
                    sb.AppendLine();
                }
            }

            // Write syntax:
            sb.AppendLine("Syntax:");
            sb.Append(" {command}");
            foreach (MemberInfo argMember in args.Values)
            {
                CommandArgAttribute argDef = CommandArgAttribute.Get(argMember);
                sb.Append(' ');

                sb.Append((argDef.IsRequired) ? '<' : '[');
                sb.Append(argDef.Name);
                sb.Append((argDef.IsRequired) ? '>' : ']');
            }
			if (argList != null)
			{
				CommandArgListAttribute arglDef = CommandArgListAttribute.Get(argList);
				sb.Append(' ');
				sb.Append((arglDef.IsRequired) ? '<' : '[');
				sb.Append(arglDef.Name);
				sb.Append("...");
				sb.Append((arglDef.IsRequired) ? '>' : ']');
			}
            foreach (MemberInfo optMember in options)
            {
                CommandOptionAttribute optDef = CommandOptionAttribute.Get(optMember);
                Type optType = CommandOptionAttribute.GetValueType(optMember);
                CommandOptionValueAttribute[] optValues = CommandOptionAttribute.GetValues(optMember);

                sb.Append(' ');

                if (optDef.IsRequired == false) sb.Append('[');

                sb.Append(this.optionMarker);
                sb.Append(optDef.Name);

                if (optValues.Length > 0)
                {
                    sb.Append(this.valueSeparator);
                    string separator = "";
                    foreach (CommandOptionValueAttribute value in optValues)
                    {
                        sb.Append(separator);
                        sb.Append(value.UserValue);
                        separator = "|";
                    }
                }
                else if (optType.Equals(typeof(Boolean)) == false)
                {
                    sb.Append(this.valueSeparator);
                    sb.Append("?");
                }

                if (optDef.IsRequired == false) sb.Append(']');
            }
            sb.AppendLine();
            sb.AppendLine();

            // Write argument details:
            foreach (MemberInfo argMember in args.Values)
            {
                CommandArgAttribute argDef = CommandArgAttribute.Get(argMember);
                CommandInfoAttribute info = CommandInfoAttribute.Get(argMember);

                sb.Append(' ');
                sb.Append(String.Concat(argDef.Name, new String(' ', 13)).Substring(0, 13));
                sb.Append(' ');
                if (info != null)
                    sb.Append(info.Description);
                else
                    sb.Append(argDef.Name);
                sb.AppendLine();
            }
            if (argList != null)
            {
				CommandArgListAttribute arglDef = CommandArgListAttribute.Get(argList);
                CommandInfoAttribute info = CommandInfoAttribute.Get(argList);

				sb.Append(' ');
				sb.Append(String.Concat(arglDef.Name, new String(' ', 13)).Substring(0, 13));
				sb.Append(' ');
				if (info != null)
                    sb.Append(info.Description);
                else
                    sb.Append("...");
                sb.AppendLine();
            }

            // Write option details:
            foreach (MemberInfo optMember in options)
            {
                CommandOptionAttribute optDef = CommandOptionAttribute.Get(optMember);
                CommandInfoAttribute info = CommandInfoAttribute.Get(optMember);
                Type optType = CommandOptionAttribute.GetValueType(optMember);
                CommandOptionValueAttribute[] optValues = CommandOptionAttribute.GetValues(optMember);

                string optname = this.optionMarker + optDef.Name;
                if (optDef.ShortName != null) optname += "|" + optDef.ShortName;

                sb.Append(' ');
                sb.Append(String.Concat(optname, new String(' ', 13)).Substring(0, 13));
                sb.Append(' ');
                if (info != null)
                    sb.Append(info.Description);
                else
                    sb.Append(optDef.Name);
				if (optValues.Length > 0)
					sb.Append(':');
				sb.AppendLine();
                foreach (CommandOptionValueAttribute value in optValues)
                {
                    sb.Append("   ");
                    sb.Append(String.Concat(value.UserValue, new String(' ', 13)).Substring(0, 13));
                    sb.Append(' ');
                    sb.Append(value.Description);
                    sb.AppendLine();
                }
            }
            sb.AppendLine();

			// Write samples:
			if (commandlineinfotype.GetCustomAttributes(typeof(CommandExampleAttribute), false).Length > 0)
			{
				sb.AppendLine("Examples:");
				CommandExampleAttribute[] samples = CommandExampleAttribute.GetValues(commandlineinfotype);
				for(int i=samples.Length-1; i>=0; i--)
				{
					CommandExampleAttribute sample = samples[i];
					sb.Append(" {command} ");
					sb.AppendLine(sample.Sample);
					if (!String.IsNullOrEmpty(sample.Description))
					{
						sb.Append("    ");
						sb.AppendLine(sample.Description);
					}
				}
				sb.AppendLine();
			}

            // Write application copyright:
            if (AssemblyCopyrightAttribute.GetCustomAttribute(entryAssembly, typeof(AssemblyCopyrightAttribute)) != null)
            {
                sb.AppendLine(((AssemblyCopyrightAttribute)AssemblyCopyrightAttribute.GetCustomAttribute(entryAssembly, typeof(AssemblyCopyrightAttribute))).Copyright.Replace("©", "(C)"));
                sb.AppendLine();
            }

			// Perform substitutions of variables:
			sb = sb.Replace("{command}", System.Diagnostics.Process.GetCurrentProcess().ProcessName);

			// Returns the whole help:
			return sb.ToString();
        }

		/// <summary>
		/// Generates and writes help information on the console.
		/// </summary>
		public void WriteHelp(Type commandlineinfotype)
		{
			Console.Write(this.GenerateHelp(commandlineinfotype));
		}

        private class CommandLineInfoHandler
        {
            internal List<string> HelpOptions = new List<string>();
            internal MemberInfo HelpHandler = null;
            internal IList<MemberInfo> ArgumentHandlers = new MemberInfo[0];
            internal MemberInfo ArgumentListHandler = null;
            internal Dictionary<String, MemberInfo> OptionHandlers = new Dictionary<string, MemberInfo>();
            internal Dictionary<String, Dictionary<string, object>> OptionValues = new Dictionary<string, Dictionary<string, object>>();
			internal int NumberOfRequiredArgs = 0;

            internal CommandLineInfoHandler(object commandlineinfo)
            {
                Type infoType = commandlineinfo.GetType();

                SortedList<int, MemberInfo> argumentHandlers = new SortedList<int, MemberInfo>();

                foreach (MemberInfo member in infoType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty))
                {
                    // Retrieve help options:
                    CommandHelpAttribute helpattr = CommandHelpAttribute.Get(member);
                    if (helpattr != null)
                    {
                        this.HelpOptions.AddRange(helpattr.HelpOptions);
                        this.HelpHandler = member;
                        continue; // Skip testing other types of handlers
                    }

                    // Retrieve argument handlers:
                    CommandArgAttribute argattr = CommandArgAttribute.Get(member);
                    if (argattr != null)
                    {
                        argumentHandlers.Add(argattr.Order, member);
						if (argattr.IsRequired) this.NumberOfRequiredArgs++;
                        continue; // Skip testing other types of handlers
                    }

                    // Retrieve argumentlist handler:
                    CommandArgListAttribute arglistattr = CommandArgListAttribute.Get(member);
                    if (arglistattr != null)
                    {
                        this.ArgumentListHandler = member;

                        // Initialize the member to empty list:
                        if (member is FieldInfo)
                        {
                            ((FieldInfo)member).SetValue(commandlineinfo, new List<string>());
                        }
                        else // Assume PropertyInfo
                        {
                            ((PropertyInfo)member).SetValue(commandlineinfo, new List<string>(), null);
                        }

						if (arglistattr.IsRequired) this.NumberOfRequiredArgs++;
						continue; // Skip testing other types of handlers
                    }

                    // Retrieve option handlers:
                    CommandOptionAttribute optattr = CommandOptionAttribute.Get(member);
                    if (optattr != null)
                    {
                        // Option values:
                        Dictionary<string, object> values = new Dictionary<string, object>();

                        // Register name, and eventual shortname:
                        this.OptionHandlers.Add(optattr.Name, member);
                        this.OptionValues.Add(optattr.Name, values);
                        if (optattr.ShortName != null)
                        {
                            this.OptionHandlers.Add(optattr.ShortName, member);
                            this.OptionValues.Add(optattr.ShortName, values);
                        }

                        // If not required, then default value is provided:
                        if (optattr.IsRequired == false)
                        {
                            // Set default value:
                            if (member is FieldInfo)
                            {
                                ((FieldInfo)member).SetValue(commandlineinfo, optattr.DefaultValue);
                            }
                            else if (member is PropertyInfo)
                            {
                                ((PropertyInfo)member).SetValue(commandlineinfo, optattr.DefaultValue, null);
                            }
                        }

                        // Retrieve optionValues:
                        CommandOptionValueAttribute[] optvalattrs = CommandOptionAttribute.GetValues(member);
                        foreach (CommandOptionValueAttribute valattr in optvalattrs)
                        {
                            values.Add(valattr.UserValue, valattr.Value);
                        }
                    }
                }

                this.ArgumentHandlers = argumentHandlers.Values;
            }
        }
    }
}
