using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Arebis.Reflection;

namespace Arebis.CodeAnalysis.Static
{
    [Serializable]
    public class ModelAssembly
    {
        private Assembly assembly;
        private ILanguageInfo languageInfo;
        private IList<ModelType> types;

        public ModelAssembly(Assembly assembly, ILanguageInfo languageInfo)
        {
            this.assembly = assembly;
            this.languageInfo = languageInfo;
            this.types = new List<ModelType>();
        }

        public Assembly Assembly
        {
            get { return this.assembly; }
        }

        public ILanguageInfo LanguageInfo
        {
            get { return this.languageInfo; }
        }

        public IList<ModelType> Types
        {
            get { return this.types; }
        }
    }
}
