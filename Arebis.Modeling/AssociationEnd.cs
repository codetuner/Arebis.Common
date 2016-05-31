using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Modeling
{
    [DataContract(Namespace = "urn:arebis.be:Modeling")]
    public class AssociationEnd : TypedFeature, IOwned<Association>
    {
        [DataMember()]
        public Association Owner
        {
            get
            {
                return Association.EndsAssociation.GetSourcesFor(this).SingleOrDefault();
            }
            set
            {
                Association.EndsAssociation.SetOrAdd(value, this);
            }
        }

        [DataMember()]
        public string Aggregation { get; set; }

        public bool IsAggregation
        {
            get { return (this.IsAggregateAggregation || this.IsCompositeAggregation); }
        }

        public bool IsCompositeAggregation
        {
            get { return ("composite".Equals(this.Aggregation, StringComparison.CurrentCultureIgnoreCase)); }
        }

        public bool IsAggregateAggregation
        {
            get { return ("aggregate".Equals(this.Aggregation, StringComparison.CurrentCultureIgnoreCase)); }
        }

        public IEnumerable<AssociationEnd> OppositeEnds
        {
            get
            {
                return this.Owner.Ends.Where(e => e != this);
            }
        }

        public override string FullName
        {
            get { return this.Owner.FullName + "->" + this.Name; }
        }
    }
}
