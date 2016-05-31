using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arebis.Reflection;
using System.Reflection;

namespace Arebis.CodeAnalysis.Static
{
    [Serializable]
    public class ModelType
    {
        private static MethodInfo[] EmptyMethodInfoSet = new MethodInfo[0];

        private ModelAssembly assembly;
        private Type runtimeType;
        private ILanguageInfo languageInfo;
        private IList<ModelMethod> methods;
        private Dictionary<MethodBase, List<MethodInfo>> interfaceMapping;

        public ModelType(ModelAssembly assembly, Type runtimeType, ILanguageInfo languageInfo)
        {
            this.assembly = assembly;
            this.runtimeType = runtimeType;
            this.languageInfo = languageInfo;
            this.methods = new List<ModelMethod>();
            this.assembly.Types.Add(this);
        }

        public ModelAssembly Assembly
        {
            get { return this.assembly; }
        }

        public Type RuntimeType
        {
            get { return this.runtimeType; }
        }

        public ILanguageInfo LanguageInfo
        {
            get { return this.languageInfo; }
        }

        public IList<ModelMethod> Methods
        {
            get { return this.methods; }
        }

        /// <summary>
        /// Returns the interface methods this method implements.
        /// </summary>
        public IEnumerable<MethodInfo> GetImplementedInterfaceMethods(ModelMethod byMethod)
        {
            if ((this.interfaceMapping == null) && (byMethod.DeclaringType.IsInterface == false))
            {
                lock (this)
                {
                    if (this.interfaceMapping == null)
                    {
                        this.interfaceMapping = new Dictionary<MethodBase, List<MethodInfo>>();

                        foreach (Type itype in this.runtimeType.GetInterfaces())
                        {
                            InterfaceMapping map = this.runtimeType.GetInterfaceMap(itype);
                            for (int i = 0; i < map.InterfaceMethods.Length; i++)
                            {
                                List<MethodInfo> list;
                                if (!this.interfaceMapping.TryGetValue(map.TargetMethods[i], out list))
                                    list = this.interfaceMapping[map.TargetMethods[i]] = new List<MethodInfo>();
                                list.Add(map.InterfaceMethods[i]);
                            }
                        }
                    }
                }
            }

            List<MethodInfo> result;
            if (byMethod.DeclaringType.IsInterface)
                return EmptyMethodInfoSet;
            else if (this.interfaceMapping.TryGetValue(byMethod.MethodBase, out result))
                return result;
            else
                return EmptyMethodInfoSet;
        }
    }
}
