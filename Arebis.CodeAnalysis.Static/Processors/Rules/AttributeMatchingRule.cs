using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;

using MethodAttributesCache = System.Collections.Generic.Dictionary<Arebis.CodeAnalysis.Static.ModelMethod, System.Collections.Generic.HashSet<System.String>>;
using TypeAttributesCache = System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.HashSet<System.String>>;
using AssemblyAttributesCache = System.Collections.Generic.Dictionary<System.Reflection.Assembly, System.Collections.Generic.HashSet<System.String>>;

namespace Arebis.CodeAnalysis.Static.Processors.Rules
{
	[CodeModelMatchingRule(
        "attributerule", 
        "Matches methods by the attributes they, their type or their assembly have.", 
        "target=Target having the attribute.|type=Typename of the attribute.")]
	public class AttributeMatchingRule : BaseMatchingRule
	{
		private const string MethodCacheKey = "AttributeMatchingRule.MethodAttributesCache";
		private const string TypeCacheKey = "AttributeMatchingRule.TypeAttributesCache";
		private const string AssemblyCacheKey = "AttributeMatchingRule.AssemblyAttributesCache";

		private string typeName;
		private RuleTarget target;

        private MethodAttributesCache methodAttributesCache;
        private TypeAttributesCache typeAttributesCache;
        private AssemblyAttributesCache assemblyAttributesCache;

		public override void Initialize(XmlNode ruleInstance)
		{
            // Initialize from XmlNode:
			this.target = (RuleTarget)Enum.Parse(typeof(RuleTarget), ruleInstance.Attributes["target"].Value, true);
			this.typeName = ruleInstance.Attributes["type"].Value;

            // Initialize caches on context:
            if (!this.Context.Properties.ContainsKey(MethodCacheKey))
                this.Context.Properties[MethodCacheKey] = new MethodAttributesCache();
            if (!this.Context.Properties.ContainsKey(TypeCacheKey))
                this.Context.Properties[TypeCacheKey] = new TypeAttributesCache();
            if (!this.Context.Properties.ContainsKey(AssemblyCacheKey))
                this.Context.Properties[AssemblyCacheKey] = new AssemblyAttributesCache();

            // Keep local references to caches:
            this.methodAttributesCache
                = (MethodAttributesCache)this.Context.Properties[MethodCacheKey];
            this.typeAttributesCache
                = (TypeAttributesCache)this.Context.Properties[TypeCacheKey];
            this.assemblyAttributesCache
                = (AssemblyAttributesCache)this.Context.Properties[AssemblyCacheKey];
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

		private bool MatchesOnMethod(ModelMethod method)
		{
			// Retrieve attributes list in cache for given method:
			HashSet<String> methodAttributes;
            if (!this.methodAttributesCache.TryGetValue(method, out methodAttributes))
			{
				methodAttributes = new HashSet<String>();
				CollectMethodAttributes(method, methodAttributes);
                lock (this.methodAttributesCache)
                {
                    this.methodAttributesCache[method] = methodAttributes;
                }
			}

			// Test match:
			return methodAttributes.Contains(this.typeName);
		}

		private bool MatchesOnType(Type type)
		{
			// Retrieve attributes list in cache for given method:
			HashSet<String> typeAttributes;
            if (!this.typeAttributesCache.TryGetValue(type, out typeAttributes))
			{
				typeAttributes = new HashSet<String>();
				CollectTypeAttributes(type, typeAttributes);
                lock (this.typeAttributesCache)
                {
                    this.typeAttributesCache[type] = typeAttributes;
                }
			}

			// Test match:
			return typeAttributes.Contains(this.typeName);
		}

		private bool MatchesOnAssembly(Assembly assembly)
		{
			// Retrieve attributes list in cache for given method:
			HashSet<String> assemblyAttributes;
            if (!this.assemblyAttributesCache.TryGetValue(assembly, out assemblyAttributes))
			{
				assemblyAttributes = new HashSet<String>();
				CollectAssemblyAttributes(assembly, assemblyAttributes);
                lock (this.assemblyAttributesCache)
                {
                    this.assemblyAttributesCache[assembly] = assemblyAttributes;
                }
			}

			// Test match:
			return assemblyAttributes.Contains(this.typeName);
		}

		private static void CollectMethodAttributes(ModelMethod method, HashSet<string> attributes)
		{
			foreach (Attribute attr in method.MethodBase.GetCustomAttributes(true))
				attributes.Add(attr.GetType().FullName);
		}

		private static void CollectTypeAttributes(Type type, HashSet<string> attributes)
		{
			foreach (Attribute attr in type.GetCustomAttributes(true))
				attributes.Add(attr.GetType().FullName);
		}

		private static void CollectAssemblyAttributes(Assembly assembly, HashSet<string> attributes)
		{
			foreach (Attribute attr in assembly.GetCustomAttributes(false))
				attributes.Add(attr.GetType().FullName);
		}
	}
}
