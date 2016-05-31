using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Xml;
using Arebis.CodeAnalysis.Static.Processors.Rules;
using System.Threading.Net2;

namespace Arebis.CodeAnalysis.Static.Processors
{
    public class RulesProcessor : IProcessor
    {
        private Dictionary<string, Type> matchingRuleTypes = new Dictionary<string, Type>();
        private XmlNode processorInstance;

        public void Initialize(XmlNode processorInstance)
        {
            this.processorInstance = processorInstance;

            // Load ruledefinition types:
            foreach (XmlNode path in processorInstance.SelectNodes(@"definitions/assembly"))
                this.RegisterMatchingRuleTypes(Assembly.LoadFrom(path.Attributes["path"].Value));
        }

        public void Process(CodeModel codeModel)
        {
            // Build RuleRunContext:
            // (The RuleRunContext allows several rule instances of the same type
            // to share a cache on a per session base.)
            RuleRunContext context = new RuleRunContext();

            // Build RuleSets:
            List<RuleSet> ruleSets = new List<RuleSet>();
            foreach (XmlNode rulesetNode in this.processorInstance.SelectNodes(@"rulesets/ruleset"))
            {
                RuleSet ruleSet = new RuleSet(rulesetNode.Attributes["name"].Value);
                ruleSets.Add(ruleSet);
                foreach (XmlNode ruleNode in rulesetNode.SelectNodes("*"))
                {
                    Type ruleType;
                    if (this.matchingRuleTypes.TryGetValue(ruleNode.Name, out ruleType))
                    {
                        BaseMatchingRule rule = (BaseMatchingRule)Activator.CreateInstance(ruleType);
                        rule.Context = context;
                        rule.Initialize(ruleNode);
                        ruleSet.Rules.Add(rule);
                    }
                }
            }

            // Apply rulesets:
            // (The next line can safely be replaced by a regular foreach as this:)
            foreach (ModelMethod method in codeModel.Methods)
            //Parallel.ForEach(codeModel.Methods, delegate(ModelMethod method)
            {
                foreach (RuleSet ruleSet in ruleSets)
                {
                    // If tag value already set, no need to evaluate this ruleset anymore...
                    if (method.Tags.Contains(ruleSet.Name))
                        continue;

                    // Test & apply ruleset:
                    if (ruleSet.Matches(method))
                    {
                        // If rule name is "_skip", move to next method:
                        if (ruleSet.Name == "_skip")
                            break;

                        // Apply ruleset:
                        method.Tags.Add(ruleSet.Name);
                    }
                }
            }//); // If using regular foreach, remove ");" here
        }

        public void RegisterMatchingRuleTypes(Assembly assembly)
        {
            foreach (Type type in assembly.GetExportedTypes())
                if (typeof(BaseMatchingRule).IsAssignableFrom(type))
                    this.RegisterMatchingRuleType(type);
        }

        public void RegisterMatchingRuleType(Type type)
        {
            if (!typeof(BaseMatchingRule).IsAssignableFrom(type))
                throw new ArgumentException("Cannot register a maching rule type: invalid type.", "type");
            foreach (CodeModelMatchingRuleAttribute attr in type.GetCustomAttributes(typeof(CodeModelMatchingRuleAttribute), false))
                this.matchingRuleTypes[attr.RuleElementName] = type;
        }
    }
}
