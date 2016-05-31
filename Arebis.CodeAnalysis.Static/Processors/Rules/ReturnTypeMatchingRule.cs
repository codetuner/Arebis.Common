using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BaseTypesCache = System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.HashSet<System.String>>;
using System.Reflection;

namespace Arebis.CodeAnalysis.Static.Processors.Rules
{
    [CodeModelMatchingRule(
        "returntyperule",
        "Matches methods by their return type.",
        "type=Base or interface type the return type should be assignable to.")]
    public class ReturnTypeMatchingRule : BaseMatchingRule
    {
        private string typeName;
        private const string BasetypesCacheKey = "ReturnTypeMatchingRule.BaseTypesCache";

        private BaseTypesCache baseTypesCache;

        public override void Initialize(System.Xml.XmlNode ruleInstance)
        {
            // Initialize from XmlNode:
            this.typeName = ruleInstance.Attributes["type"].Value;

            // Initialize cache on context:
            if (!this.Context.Properties.ContainsKey(BasetypesCacheKey))
                this.Context.Properties[BasetypesCacheKey] = new BaseTypesCache();

            // Keep local reference to cache:
            this.baseTypesCache
                = (BaseTypesCache)this.Context.Properties[BasetypesCacheKey];
        }

        public override bool Matches(ModelMethod method)
        {
            Type returnType = null;
            if (method.MethodBase is MethodInfo)
                returnType = ((MethodInfo)method.MethodBase).ReturnType;

            return this.Matches(returnType ?? typeof(void));
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

            foreach (Type iface in type.GetInterfaces())
                CollectBaseTypes(iface, basetypenames);
        }
    }
}
