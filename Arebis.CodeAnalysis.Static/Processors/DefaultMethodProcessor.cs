using System;
using System.Linq;

namespace Arebis.CodeAnalysis.Static.Processors
{
    /// <summary>
    /// Processor connects the methods tagged with "defaultmethod"
    /// to their constructors.
    /// </summary>
    public class DefaultMethodProcessor : IProcessor
    {
        private const string DefaultMethodTag = "defaultmethod";

        public void Initialize(System.Xml.XmlNode processorInstance)
        {
        }

        public void Process(CodeModel codeModel)
        {
            foreach (ModelMethod method in codeModel.Methods.WhereTagsContains(DefaultMethodTag))
            {
                foreach (ModelMethod constructor in method.Type.Methods.Where(m => m.IsConstructor))
                {
                    constructor.CallsMethods.Add(method);
                }
            }
        }
    }
}
