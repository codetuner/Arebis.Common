using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using BaseTypesCache=System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.HashSet<System.String>>;

namespace Arebis.CodeAnalysis.Static.Processors.Rules
{
	[CodeModelMatchingRule(
        "basetyperule", 
        "Matches methods on subtypes of the given type.",
        "type=Name of the base type.|includeinterfaces=Whether the type can be an interface.")]
	public class BaseTypeMatchingRule : BaseMatchingRule
	{
		private const string BasetypesCacheKey = "BaseTypeMatchingRule.BaseTypesCache";

		private string typeName;
        private bool includeInterfaces = false;

        private BaseTypesCache baseTypesCache;

		public override void Initialize(XmlNode ruleInstance)
		{
            // Initialize from XmlNode:
            this.typeName = ruleInstance.Attributes["type"].Value;
            if (ruleInstance.Attributes["includeinterfaces"] != null)
                this.includeInterfaces = Convert.ToBoolean(ruleInstance.Attributes["includeinterfaces"].Value);

            // Initialize cache on context:
            if (!this.Context.Properties.ContainsKey(BasetypesCacheKey))
                this.Context.Properties[BasetypesCacheKey] = new BaseTypesCache();

            // Keep local reference to cache:
            this.baseTypesCache
                = (BaseTypesCache)this.Context.Properties[BasetypesCacheKey];
        }

		public override bool Matches(ModelMethod method)
		{
			return this.Matches(method.DeclaringType);
		}
		
		private bool Matches(Type type)
		{
			// Retrieve basetypes list in cache for given type:
			HashSet<String> baseTypes;
            if (!this.baseTypesCache.TryGetValue(type, out baseTypes))
			{
                baseTypes = new HashSet<String>();
				CollectBaseTypes(type, baseTypes);
                lock (this.baseTypesCache)
                {
                    this.baseTypesCache[type] = baseTypes;
                }
			}

			// Test match:
			return baseTypes.Contains(this.typeName);
		}

		private void CollectBaseTypes(Type type, HashSet<String> basetypenames)
		{
			if (type.FullName != null)
				basetypenames.Add(type.FullName);
            if (type.BaseType != null)
                CollectBaseTypes(type.BaseType, basetypenames);

            if (this.includeInterfaces)
                foreach (Type iface in type.GetInterfaces())
                    CollectBaseTypes(iface, basetypenames);
		}
	}
}
