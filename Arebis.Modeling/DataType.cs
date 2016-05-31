using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Modeling
{
    [DataContract(Namespace = "urn:arebis.be:Modeling")]
    public class DataType : ModelElement, IModelType
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsAbstract { get; set; }
    }
}
