using System;

namespace Arebis.CodeAnalysis.Static.Processors
{
    /// <summary>
    /// A processor that connects overriding methods to
    /// their base methods having the "baseimplementation" tag.
    /// </summary>
    public class VirtualMethodProcessor : IProcessor
    {
        private const string BaseImplementationTag = "baseimplementation";

        public void Initialize(System.Xml.XmlNode processorInstance)
        {
        }

        public void Process(CodeModel codeModel)
        {
            foreach (ModelMethod method in codeModel.Methods)
            {
                if (method.IsOverride)
                {
                    ModelMethod baseMethod = codeModel.Methods.ForMethodBase(method.BaseMethodDefinition);
                    if (baseMethod != null && baseMethod.HasAnyOfTags(BaseImplementationTag))
                    {
                        baseMethod.CallsMethods.Add(method);
                    }
                }
            }
        }
    }
}
