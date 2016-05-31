using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Arebis.CodeAnalysis.Static.Processors.Rules
{
	[CodeModelMatchingRule(
        "interfacerule", 
        "Matches methods by the fact they implement a method of the given interface.",
        "type=Name of the interface.")]
	public class InterfaceTypeMatchingRule : BaseMatchingRule
	{
		private string interfaceName;

		public override void Initialize(XmlNode ruleInstance)
		{
            // Initialize from XmlNode:
            this.interfaceName = ruleInstance.Attributes["type"].Value;
        }

        public override bool Matches(ModelMethod method)
        {
            foreach (MethodInfo ifaceMethod in method.ImplementedInterfaceMethods)
            {
                if (ifaceMethod.DeclaringType.FullName == this.interfaceName)
                    return true;
            }

            return false;
        }
	}
}
