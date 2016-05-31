using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Modeling
{
    [DataContract(Namespace = "urn:arebis.be:Modeling")]
    public class Association : ModelElement
    {
        public static readonly AssociationProperty<Association, AssociationEnd> EndsAssociation = new AssociationProperty<Association, AssociationEnd>("AssociationEnds", AssociationMultiplicity.Single, AssociationMultiplicity.Multiple);
        private ICollection<AssociationEnd> EndsCollection = null;

        [DataMember]
        public ICollection<AssociationEnd> Ends
        {
            get
            {
                if (this.EndsCollection == null)
                    this.EndsCollection = EndsAssociation.GetTargetCollectionFor(this);
                return this.EndsCollection;
            }
            set
            {
                this.Ends.Clear();
                foreach (var item in value)
                    this.Ends.Add(item);
            }
        }

        /// <summary>
        /// Find subitems matching the given predicate.
        /// </summary>
        public override IEnumerable<ModelBase> Find(Predicate<ModelBase> predicate, bool includeFeatures)
        {
            foreach (var item in base.Find(predicate, includeFeatures))
                yield return item;

            if (includeFeatures)
            {
                foreach (var item in this.Ends)
                    foreach (var subitem in item.Find(predicate, includeFeatures))
                        yield return subitem;
            }
        }

        public override void Print(System.IO.TextWriter writer, string prefix)
        {
            base.Print(writer, prefix);
            foreach (var item in this.Ends)
                item.Print(writer, prefix + "  ");
        }
    }
}
