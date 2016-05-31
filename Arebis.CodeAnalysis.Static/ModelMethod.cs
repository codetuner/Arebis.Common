using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Arebis.Reflection;

namespace Arebis.CodeAnalysis.Static
{
    /// <summary>
    /// A method node in a network of method calls.
    /// </summary>
    [Serializable]
    public class ModelMethod
    {
        private ModelType type;
        private ILanguageInfo languageInfo;
        private MethodBase methodBase;
        private string signature;
        private string signatureWithParamNames;
        private MethodCallsList callsMethods;
        private HashSet<ModelMethod> calledByMethods = new HashSet<ModelMethod>();
        private List<object> tags = new List<object>();
        private int cachedHashCode;
        private string cachedToString;

        [NonSerialized]
        private HashSet<ModelMethod> allCallsMethods = null;
        [NonSerialized]
        private HashSet<ModelMethod> allCalledByMethods = null;

        public ModelMethod(ModelType type, MethodBase methodBase, ILanguageInfo languageInfo)
        {
            this.callsMethods = new MethodCallsList(this);
            this.type = type;
            this.languageInfo = languageInfo;
            this.methodBase = methodBase;
            this.type.Methods.Add(this);
        }

        public ModelType Type
        {
            get { return this.type; }
        }

        public ILanguageInfo LanguageInfo
        {
            get { return this.languageInfo; }
        }

        /// <summary>
        /// Name of the method.
        /// </summary>
        public string Name
        {
            get { return this.methodBase.Name; }
        }

        /// <summary>
        /// Declaring type of the method.
        /// </summary>
        public Type DeclaringType
        {
            get { return this.methodBase.DeclaringType; }
        }

        /// <summary>
        /// Signature of the method expressed in it's LanguageInfo, excluding parameter names.
        /// </summary>
        public string Signature
        {
            get
            {
                if (this.signature == null)
                {
                    StringBuilder signatureBuilder = new StringBuilder();

                    Type returnType = null;
                    if (this.methodBase is MethodInfo)
                        returnType = ((MethodInfo)this.methodBase).ReturnType;

                    signatureBuilder.Append(this.methodBase.Name);

                    signatureBuilder.Append('(');
                    string separator = "";
                    foreach (ParameterInfo param in this.methodBase.GetParameters())
                    {
                        if (param.IsRetval)
                        {
                            returnType = param.ParameterType;
                            continue;
                        }

                        signatureBuilder.Append(separator);

                        if (param.IsOut && !param.ParameterType.IsByRef)
                            signatureBuilder.Append("out ");

                        signatureBuilder.Append(this.languageInfo.GetFiendlyName(param.ParameterType));
                        separator = ", ";
                    }
                    if (returnType == null)
                    {
                        signatureBuilder.Append(")");
                    }
                    else
                    {
                        signatureBuilder.Append(") : ");
                        signatureBuilder.Append(this.languageInfo.GetFiendlyName(returnType));
                    }

                    this.signature = signatureBuilder.ToString();
                }
                return this.signature;
            }
        }

        /// <summary>
        /// Signature of the method expressed in it's LanguageInfo, including parameter names.
        /// </summary>
        public string SignatureWithParamNames
        {
            get
            {
                if (this.signatureWithParamNames == null)
                {
                    StringBuilder signatureBuilder = new StringBuilder();

                    Type returnType = null;
                    if (!this.methodBase.IsConstructor)
                        returnType = ((MethodInfo)this.methodBase).ReturnType;

                    signatureBuilder.Append(this.methodBase.Name);

                    signatureBuilder.Append('(');
                    string separator = "";
                    foreach (ParameterInfo param in this.methodBase.GetParameters())
                    {
                        if (param.IsRetval)
                        {
                            returnType = param.ParameterType;
                            continue;
                        }

                        signatureBuilder.Append(separator);

                        if (param.IsOut && !param.ParameterType.IsByRef)
                            signatureBuilder.Append("out ");

                        signatureBuilder.Append(this.languageInfo.GetFiendlyName(param.ParameterType));
                        signatureBuilder.Append(' ');
                        signatureBuilder.Append(param.Name);
                        separator = ", ";
                    }
                    if (returnType == null)
                    {
                        signatureBuilder.Append(")");
                    }
                    else
                    {
                        signatureBuilder.Append(") : ");
                        signatureBuilder.Append(this.languageInfo.GetFiendlyName(returnType));
                    }

                    this.signatureWithParamNames = signatureBuilder.ToString();
                }
                return this.signatureWithParamNames;
            }
        }

        /// <summary>
        /// Tags of the method.
        /// </summary>
        public List<object> Tags
        {
            get { return this.tags; }
        }

        /// <summary>
        /// MethodBase object.
        /// </summary>
        public MethodBase MethodBase
        {
            get { return this.methodBase; }
        }

        /// <summary>
        /// The base definition of the method (the first declaration of a virtual method).
        /// </summary>
        public MethodBase BaseMethodDefinition
        {
            get 
            {
                MethodInfo asInfo = this.methodBase as MethodInfo;
                if (asInfo == null)
                    return this.methodBase;
                else
                    return asInfo.GetBaseDefinition();
            }
        }


        public IEnumerable<MethodInfo> ImplementedInterfaceMethods
        {
            get { return this.type.GetImplementedInterfaceMethods(this); }
        }

        /// <summary>
        /// The methods this method calls.
        /// </summary>
        public ICollection<ModelMethod> CallsMethods
        {
            get { return this.callsMethods; }
        }

        /// <summary>
        /// The methods that call this method.
        /// </summary>
        public IEnumerable<ModelMethod> CalledByMethods
        {
            get { return this.calledByMethods; }
        }

        /// <summary>
        /// Whether this method has at least one of the given tags.
        /// </summary>
        public bool HasAnyOfTags(params object[] tags)
        {
            foreach (object tag in tags)
                if (this.tags.Contains(tag))
                    return true;
            return false;
        }

        /// <summary>
        /// Get all methods called by this one, directly and indirectly.
        /// </summary>
        public IEnumerable<ModelMethod> GetAllCallsMethods()
        {
            if (this.allCallsMethods == null)
            {
                this.allCallsMethods = new HashSet<ModelMethod>();
                this.BuildAllCallsMethods(this.allCallsMethods, this);
            }
            return new List<ModelMethod>(this.allCallsMethods);
        }

        private void BuildAllCallsMethods(HashSet<ModelMethod> calls, ModelMethod parent)
        {
            foreach (ModelMethod m in parent.CallsMethods)
            {
                if (!calls.Contains(m))
                {
                    calls.Add(m);
                    this.BuildAllCallsMethods(calls, m);
                }
            }
        }

        /// <summary>
        /// Get all methods that call this one directly or indirectly.
        /// </summary>
        public IEnumerable<ModelMethod> GetAllCalledByMethods()
        {
            if (this.allCalledByMethods == null)
            {
                this.allCalledByMethods = new HashSet<ModelMethod>();
                this.BuildAllCalledByMethods(this.allCalledByMethods, this);
            }
            return new List<ModelMethod>(this.allCalledByMethods);
        }

        private void BuildAllCalledByMethods(HashSet<ModelMethod> calls, ModelMethod parent)
        {
            foreach (ModelMethod m in parent.CalledByMethods)
            {
                if (!calls.Contains(m))
                {
                    calls.Add(m);
                    this.BuildAllCalledByMethods(calls, m);
                }
            }
        }

        /// <summary>
        /// The list of paths from this method to the given called target method.
        /// </summary>
        public List<List<ModelMethod>> PathsTo(ModelMethod targetMethod)
        {
            List<List<ModelMethod>> paths = new List<List<ModelMethod>>();
            paths.Add(new List<ModelMethod>() { this });
            return BuildPathsTo(paths, targetMethod);
        }

        private List<List<ModelMethod>> BuildPathsTo(List<List<ModelMethod>> paths, ModelMethod targetMethod)
        {
            List<List<ModelMethod>> newPaths = new List<List<ModelMethod>>();
            foreach (var path in paths)
            {
                if (path.Last().Equals(targetMethod))
                {
                    newPaths.Add(path);
                }
                else
                {
                    foreach (var callsMethod in path.Last().CallsMethods)
                    {
                        // Skip recursive calls:
                        if (path.Contains(callsMethod))
                            continue;

                        if (callsMethod.Equals(targetMethod))
                        {
                            List<ModelMethod> newPath = new List<ModelMethod>(path);
                            newPath.Add(callsMethod);
                            newPaths.Add(newPath);
                        }
                        else if (callsMethod.GetAllCallsMethods().Contains(targetMethod))
                        {
                            List<ModelMethod> newPath = new List<ModelMethod>(path);
                            newPath.Add(callsMethod);

                            newPaths.AddRange(BuildPathsTo(new List<List<ModelMethod>>() { newPath }, targetMethod));
                        }
                    }
                }
            }

            return newPaths;
        }

        /// <summary>
        /// Whether the method is an instance constructor method.
        /// </summary>
        public bool IsConstructor
        {
            get { return this.MethodBase.IsConstructor; }
        }

        /// <summary>
        /// Whether the method is a static constructor method.
        /// </summary>
        public bool IsStaticConstructor
        {
            get 
            { 
                return (this.methodBase.IsStatic 
                     && this.methodBase.IsSpecialName 
                     && this.methodBase.Name == ".cctor"); 
            }
        }

        /// <summary>
        /// Whether this method is an override.
        /// The base method can then be retrieved using the BaseMethodDefinition property.
        /// </summary>
        public bool IsOverride
        {
            get { return (this.BaseMethodDefinition != this.methodBase);  }
        }

        /// <summary>
        /// Whether the method is an accessor method.
        /// </summary>
        public bool IsAccessor
        {
            get { return (this.MethodBase.IsSpecialName && (this.MethodBase.Name.StartsWith("get_") || this.MethodBase.Name.StartsWith("set_"))); }
        }

        /// <summary>
        /// Whether the method is a get accessor method.
        /// </summary>
        public bool IsGetAccessor
        {
            get { return (this.MethodBase.IsSpecialName && this.MethodBase.Name.StartsWith("get_")); }
        }

        /// <summary>
        /// Whether the method is a set accessor method.
        /// </summary>
        public bool IsSetAccessor
        {
            get { return (this.MethodBase.IsSpecialName && this.MethodBase.Name.StartsWith("set_")); }
        }

        /// <summary>
        /// Whether the method is an operator overload.
        /// </summary>
        public bool IsOperator
        {
            get { return (this.MethodBase.IsSpecialName && this.MethodBase.Name.StartsWith("op_")); }
        }

        /// <summary>
        /// Whether the method is an anonymous delegate.
        /// </summary>
        public bool IsAnonymous
        {
            get { return this.methodBase.Name.Contains("<") || this.methodBase.DeclaringType.Name.Contains("<"); }
        }

        /// <summary>
        /// Whether this method is public.
        /// </summary>
        public bool IsPublic
        {
            get { return this.methodBase.IsPublic; }
        }

        public bool IsProtected
        {
            get 
            { 
                return ((this.methodBase.Attributes & MethodAttributes.Family) == MethodAttributes.Family);
            }
        }

        public bool IsInternal
        {
            get 
            {
                return ((this.methodBase.Attributes & MethodAttributes.Assembly) == MethodAttributes.Assembly)
                    || ((this.methodBase.Attributes & MethodAttributes.FamORAssem) == MethodAttributes.FamORAssem);
            }
        }

        /// <summary>
        /// Whether this method is static.
        /// </summary>
        public bool IsStatic
        {
            get { return this.methodBase.IsStatic; }
        }

        /// <summary>
        /// Whether this method is virtual.
        /// </summary>
        public bool IsVirtual
        {
            get { return this.methodBase.IsVirtual; }
        }

        /// <summary>
        /// Compares whether two ModelMethod objects reference the same CLR method.
        /// </summary>
        public override bool Equals(object obj)
        {
            ModelMethod other = obj as ModelMethod;
            if (Object.ReferenceEquals(other, null))
                return false;
            else
                return this.MethodBase.Equals(other.MethodBase);
        }

        public override int GetHashCode()
        {
            if (this.cachedHashCode == 0)
                this.cachedHashCode = this.MethodBase.GetHashCode();
            return this.cachedHashCode;
        }

        public override string ToString()
        {
            if (this.cachedToString == null)
            {
                this.cachedToString = String.Format("{0}.{1}({2})",
                    this.DeclaringType,
                    this.Name,
                    new String('.', this.MethodBase.GetParameters().Length));
            }
            return this.cachedToString;
        }

        /// <summary>
        /// Represents a list of directly called methods.
        /// </summary>
        internal class MethodCallsList : ICollection<ModelMethod>
        {
            private ModelMethod owner;
            private HashSet<ModelMethod> innerList = new HashSet<ModelMethod>();

            public MethodCallsList(ModelMethod owner)
            {
                this.owner = owner;
            }

            #region ICollection<ModelMethod> Members

            public void Add(ModelMethod item)
            {
                if (this.innerList.Add(item))
                    item.calledByMethods.Add(owner);
            }

            public bool Remove(ModelMethod item)
            {
                bool removed = this.innerList.Remove(item);
                if (removed) item.calledByMethods.Remove(owner);
                return removed;
            }

            public void Clear()
            {
                while (this.innerList.Count > 0)
                {
                    ModelMethod item = this.innerList.First();
                    this.Remove(item);
                }
            }

            public bool Contains(ModelMethod item)
            {
                return this.innerList.Contains(item);
            }

            public void CopyTo(ModelMethod[] array, int arrayIndex)
            {
                this.innerList.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return this.innerList.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            #endregion

            #region IEnumerable<ModelMethod> Members

            public IEnumerator<ModelMethod> GetEnumerator()
            {
                return this.innerList.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((System.Collections.IEnumerable)this.innerList).GetEnumerator();
            }

            #endregion
        }
    }
}
