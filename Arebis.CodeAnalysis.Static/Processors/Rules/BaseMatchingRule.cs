using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Arebis.CodeAnalysis.Static.Processors.Rules
{
    /// <summary>
    /// Base class for implementing RuleProcessor matching rules.
    /// </summary>
	public abstract class BaseMatchingRule
	{
		private RuleRunContext context;

        /// <summary>
        /// The context the rule runs in.
        /// </summary>
		public RuleRunContext Context
		{
			get { return this.context; }
			set { this.context = value; }
		}

        /// <summary>
        /// Initializes the rule instance with it's XML definition.
        /// </summary>
		public abstract void Initialize(XmlNode ruleInstance);

        /// <summary>
        /// Whether the given method positively matches this rule.
        /// </summary>
		public abstract bool Matches(ModelMethod method);
	}
}
