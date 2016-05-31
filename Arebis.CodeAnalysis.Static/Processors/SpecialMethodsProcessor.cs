using System;

namespace Arebis.CodeAnalysis.Static.Processors
{
    /// <summary>
    /// Processor that tags special methods as constructors, 
    /// operator overloads and property accessor methods.
    /// </summary>
    public class SpecialMethodsProcessor : IProcessor
    {
        public void Initialize(System.Xml.XmlNode processorInstance)
        {
        }

        public void Process(CodeModel codeModel)
        {
            foreach (ModelMethod method in codeModel.Methods)
            {
                if (method.IsConstructor)
                {
                    method.Tags.Add("specialmethod");
                    method.Tags.Add("instanceconstructor");
                    method.Tags.Add("constructor");
                }
                else if (method.IsStaticConstructor)
                {
                    method.Tags.Add("specialmethod");
                    method.Tags.Add("staticconstructor");
                    method.Tags.Add("constructor");
                }
                else if (method.IsOperator)
                {
                    method.Tags.Add("specialmethod");
                    method.Tags.Add("operator");
                }
                else if (method.IsGetAccessor)
                {
                    method.Tags.Add("specialmethod");
                    method.Tags.Add("accessor");
                    method.Tags.Add("get_accessor");
                }
                else if (method.IsSetAccessor)
                {
                    method.Tags.Add("specialmethod");
                    method.Tags.Add("accessor");
                    method.Tags.Add("set_accessor");
                }
                else if (method.IsAnonymous)
                {
                    method.Tags.Add("anonymous");
                }
            }
        }
    }
}
