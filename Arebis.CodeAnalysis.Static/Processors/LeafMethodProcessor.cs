using System;

namespace Arebis.CodeAnalysis.Static.Processors
{
    /// <summary>
    /// Processor breaking connection of methods tagged with "leafmethod"
    /// to calling methods.
    /// </summary>
    public class LeafMethodProcessor : IProcessor
    {
        private const string LeafMethodTag = "leafmethod";

        public void Initialize(System.Xml.XmlNode processorInstance)
        {
        }

        public void Process(CodeModel codeModel)
        {
            foreach (ModelMethod method in codeModel.Methods.WhereTagsContains(LeafMethodTag))
            {
                method.CallsMethods.Clear();
            }
        }
    }
}
