using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;

namespace Arebis.CodeAnalysis.Static.Processors.Rules
{
	[CodeModelMatchingRule(
        "modifierrule", 
        "Matches methods by their modifiers or by their types modifiers.",
        "target=Target of the modifier.|modifiers=Comma-separated list of modifiers.")]
	public class ModifierMatchingRule : BaseMatchingRule
	{
		private RuleTarget target;
		private MethodAttributes methodAttributes;
		private TypeAttributes typeAttributes;
        private bool reverse;

		public override void Initialize(XmlNode ruleInstance)
		{
			this.target = (RuleTarget)Enum.Parse(typeof(RuleTarget), ruleInstance.Attributes["target"].Value, true);
			switch(this.target)
			{
				case RuleTarget.Method:
					InitializeMethod(ruleInstance);
					break;
				case RuleTarget.Type:
					InitializeType(ruleInstance);
					break;
				default:
					throw new InvalidOperationException(String.Format("Invalid target '{0}' for {1}.", this.target, this.GetType().Name));
			}

            if (ruleInstance.Attributes["reverse"] != null)
                this.reverse = Convert.ToBoolean(ruleInstance.Attributes["reverse"].Value);
        }

		private void InitializeMethod(XmlNode ruleInstance)
		{
			string modifierstring = ruleInstance.Attributes["modifiers"].Value;
			foreach (string s in modifierstring.Split(','))
				this.methodAttributes =
					this.methodAttributes
					| (MethodAttributes)Enum.Parse(typeof(MethodAttributes), s.Trim(), true);
		}

		private void InitializeType(XmlNode ruleInstance)
		{
			string modifierstring = ruleInstance.Attributes["modifiers"].Value;
			foreach (string s in modifierstring.Split(','))
				this.typeAttributes = 
					this.typeAttributes 
					| (TypeAttributes)Enum.Parse(typeof(TypeAttributes), s.Trim(), true);
		}

		public override bool Matches(ModelMethod method)
		{
			switch(this.target)
			{
				case RuleTarget.Method:
					return this.MatchesOnMethod(method);
				case RuleTarget.Type:
					return this.MatchesOnType(method.DeclaringType);
				default:
					throw new InvalidOperationException(String.Format("Invalid target '{0}' for {1}.", this.target, this.GetType().Name));
			}
		}

		private bool MatchesOnMethod(ModelMethod method)
		{
			return this.reverse ^ ((method.MethodBase.Attributes & this.methodAttributes) == this.methodAttributes);
		}

		private bool MatchesOnType(Type type)
		{
            return this.reverse ^ ((type.Attributes & this.typeAttributes) == this.typeAttributes);
		}
	}
}
