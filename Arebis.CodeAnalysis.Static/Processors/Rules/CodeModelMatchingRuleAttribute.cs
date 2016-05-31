using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.CodeAnalysis.Static.Processors.Rules
{
    /// <summary>
    /// Marks a class as a CodeModelMatchingRule (the class must also inherit from BaseMatchingRule),
    /// defines the element name of the rule and documentation of the rule.
    /// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CodeModelMatchingRuleAttribute : Attribute
	{
		private string ruleElementName;
		private Dictionary<string, string> attributes = new Dictionary<string, string>();
        private string description;

		public CodeModelMatchingRuleAttribute(string ruleElementName)
			: this(ruleElementName, null, null)
		{ }

		public CodeModelMatchingRuleAttribute(string ruleElementName, string description, string attributeDocumentation)
		{
			this.ruleElementName = ruleElementName;
            this.description = description;
			if (attributeDocumentation != null)
			{
				foreach (string attrDoc in attributeDocumentation.Split('|'))
				{
					string[] attrAndDoc = attrDoc.Split(new char[] { '=' }, 2);
					this.attributes[attrAndDoc[0].Trim()] = attrAndDoc[1].Trim();
				}
			}
		}

		public string RuleElementName
		{
			get { return this.ruleElementName; }
		}

        public string Description
        {
            get { return this.description; }
        }

		public Dictionary<string, string> Attributes
		{
			get { return this.attributes; }
		}
	}
}
