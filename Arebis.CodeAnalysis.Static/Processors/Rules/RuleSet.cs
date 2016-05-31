using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.CodeAnalysis.Static.Processors.Rules
{
	public class RuleSet
	{
		private string name;
		private List<BaseMatchingRule> rules = new List<BaseMatchingRule>();

		public RuleSet(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return this.name; }
		}

		public List<BaseMatchingRule> Rules
		{
			get { return this.rules; }
		}

		public bool Matches(ModelMethod method)
		{
			foreach (BaseMatchingRule rule in this.rules)
				if (!rule.Matches(method))
					return false;
			return true;
		}
	}
}
