using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Modeling
{
    [DataContract(Namespace = "urn:arebis.be:Modeling")]
    [KnownType(typeof(Package))]
    [KnownType(typeof(Class))]
    [KnownType(typeof(Association))]
    [KnownType(typeof(Relationship))]
    [KnownType(typeof(DataType))]
    public abstract class ModelElement : ModelBase, IOwned<Package>
    {
        [DataMember()]
        public Package Owner
        {
            get
            {
                return Package.MembersAssociation.GetSourcesFor(this).SingleOrDefault();
            }
            set
            {
                Package.MembersAssociation.SetOrAdd(value, this);
            }
        }

        public override string FullName
        {
            get 
            {
                if (this.Owner != null)
                    return this.Owner.FullName + "::" + this.Name;
                else
                    return this.Name;
            }
        }
    }
}
