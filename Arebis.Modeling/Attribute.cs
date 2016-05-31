using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Modeling
{
    [DataContract(Namespace = "urn:arebis.be:Modeling")]
    public class Attribute : TypedFeature, IOwned<Class>
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Class Owner
        {
            get
            {
                return Class.AttributesAssociation.GetSourcesFor(this).SingleOrDefault();
            }
            set
            {
                Class.AttributesAssociation.SetOrAdd(value, this);
            }
        }

        [DataMember]
        public string InitialValue { get; set; }

        public override void Print(System.IO.TextWriter writer, string prefix)
        {
            if (this.InitialValue == null)
                writer.WriteLine(prefix + "{0}: {1} : {2}", this.GetType().Name, this, this.Type);
            else
                writer.WriteLine(prefix + "{0}: {1} : {2} = {3}", this.GetType().Name, this, this.Type, this.InitialValue);
        }

        public override string FullName
        {
            get { return this.Owner.FullName + "." + this.Name; }
        }
    }
}
