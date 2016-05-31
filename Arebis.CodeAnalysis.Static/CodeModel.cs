using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.CodeAnalysis.Static
{
    /// <summary>
    /// A model of the analyzed code.
    /// </summary>
    [Serializable]
    public class CodeModel
    {
        public CodeModel()
        {
            this.Assemblies = new List<ModelAssembly>();
            this.Types = new List<ModelType>();
            this.Methods = new List<ModelMethod>();
        }

        public CodeModel(
            IEnumerable<ModelAssembly> assemblies,
            IEnumerable<ModelType> types,
            IEnumerable<ModelMethod> methods)
        {
            this.Assemblies = new List<ModelAssembly>(assemblies);
            this.Types = new List<ModelType>(types);
            this.Methods = new List<ModelMethod>(methods);
        }

        public IList<ModelAssembly> Assemblies { get; private set; }
        
        public IList<ModelType> Types { get; private set; }
        
        public IList<ModelMethod> Methods { get; private set; }
    }
}
