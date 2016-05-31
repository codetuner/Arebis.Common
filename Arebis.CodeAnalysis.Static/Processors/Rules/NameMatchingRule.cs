using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Reflection;

namespace Arebis.CodeAnalysis.Static.Processors.Rules
{
	[CodeModelMatchingRule(
        "namerule", 
        "Matches methods by their name, their types name or their assemblies name.",
        "target=Target of the name.|like=Wildcard expression the name should match.|match=Regular expression the name should match.")]
	public class NameMatchingRule : BaseMatchingRule
	{
		private RuleTarget target;
		private Regex match;

		public override void Initialize(XmlNode ruleInstance)
		{
			this.target = (RuleTarget)Enum.Parse(typeof(RuleTarget), ruleInstance.Attributes["target"].Value, true);
            if (ruleInstance.Attributes["match"] != null)
                this.match = new Regex(ruleInstance.Attributes["match"].Value, RegexOptions.Compiled);
            else if (ruleInstance.Attributes["like"] != null)
                this.match = GetLikeRegex(ruleInstance.Attributes["like"].Value, RegexOptions.Compiled);
            else
                throw new InvalidOperationException(String.Format("Name matching rule must have either match or like attribute in \"{0}\".", ruleInstance.OuterXml));
		}

        /// <summary>
        /// Returns a RegEx to match the given pattern with wildcards * and ?.
        /// </summary>
        public static Regex GetLikeRegex(string pattern, RegexOptions options)
        {
            pattern = Regex.Escape(pattern);
            pattern = "^" + pattern.Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return new Regex(pattern, options);
        }

		public override bool Matches(ModelMethod method)
		{
			switch (this.target)
			{
				case RuleTarget.Method:
					return this.MatchesOnMethod(method);
				case RuleTarget.Type:
					return this.MatchesOnType(method.DeclaringType);
				case RuleTarget.Assembly:
					return this.MatchesOnAssembly(method.DeclaringType.Assembly);
				default:
					throw new InvalidOperationException(String.Format("Invalid target '{0}' for {1}.", this.target, this.GetType().Name));
			}
		}

		public bool MatchesOnMethod(ModelMethod method)
		{
			return (this.match.IsMatch(method.Name));
		}

		public bool MatchesOnType(Type type)
		{
			return ((type.FullName != null) && (this.match.IsMatch(type.FullName)));
		}

		public bool MatchesOnAssembly(Assembly assembly)
		{
			return (this.match.IsMatch(assembly.GetName().Name));
		}
	}
}
