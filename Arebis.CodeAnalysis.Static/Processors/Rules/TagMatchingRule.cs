using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Arebis.CodeAnalysis.Static.Processors.Rules
{
	[CodeModelMatchingRule(
        "tagrule", 
        "Matches methods by their tags.",
        "name=Name of the expected tag.|reverse=True to reverse the match.")]
	public class TagMatchingRule : BaseMatchingRule
	{
		private string tagName;
        private bool reverse;

		public override void Initialize(XmlNode ruleInstance)
		{
			this.tagName = ruleInstance.Attributes["name"].Value;
            if (ruleInstance.Attributes["reverse"] != null)
                this.reverse = Convert.ToBoolean(ruleInstance.Attributes["reverse"].Value);
		}

		public override bool Matches(ModelMethod method)
		{
            return this.reverse ^ method.Tags.Contains(this.tagName);
		}
	}
}
