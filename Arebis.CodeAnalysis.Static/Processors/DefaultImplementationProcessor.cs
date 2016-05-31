using System;
using System.Reflection;

namespace Arebis.CodeAnalysis.Static.Processors
{
    /// <summary>
    /// Processor that connects methods tagged with "defaultimplementation"
    /// to their interface declaration.
    /// </summary>
    public class DefaultImplementationProcessor : IProcessor
    {
        private const string DefaultImplementationTag = "defaultimplementation";

        public void Initialize(System.Xml.XmlNode processorInstance)
        {
        }

        public void Process(CodeModel codeModel)
        {
            foreach (ModelMethod method in codeModel.Methods.WhereTagsContains(DefaultImplementationTag))
            {
                foreach (MethodInfo imethodInfo in method.ImplementedInterfaceMethods)
                {
                    ModelMethod imethod = codeModel.Methods.ForMethodBase(imethodInfo);
                    if (imethod != null)
                    {
                        imethod.CallsMethods.Add(method);
                    }
                }
            }
        }
    }
}
