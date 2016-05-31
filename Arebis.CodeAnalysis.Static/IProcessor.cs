using System;
using System.Xml;
using System.Collections.Generic;

namespace Arebis.CodeAnalysis.Static
{
    public interface IProcessor
    {
        void Initialize(XmlNode processorInstance);

        void Process(CodeModel codeModel);
    }
}
