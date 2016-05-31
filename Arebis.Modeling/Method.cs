using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Modeling
{
    [DataContract(Namespace = "urn:arebis.be:Modeling")]
    public class Method : TypedFeature, IOwned<Class>
    {
        [DataMember()]
        public Class Owner
        {
            get
            {
                return Class.MethodsAssociation.GetSourcesFor(this).SingleOrDefault();
            }
            set
            {
                Class.MethodsAssociation.SetOrAdd(value, this);
            }
        }

        public override string FullName
        {
            get { return this.Owner.FullName + "." + this.Name + "()"; }
        }
    }
}
