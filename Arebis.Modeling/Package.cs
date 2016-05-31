using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Modeling
{
    [DataContract(Namespace = "urn:arebis.be:Modeling")]
    public class Package : ModelElement
    {
        public static readonly AssociationProperty<Package, ModelElement> MembersAssociation = new AssociationProperty<Package, ModelElement>("PackageMembers", AssociationMultiplicity.Single, AssociationMultiplicity.Multiple);
        private ICollection<ModelElement> MembersCollection = null;

        /// <summary>
        /// The package this package is owned by (null if this package is owned by a model).
        /// </summary>
        public Package Parent
        {
            get { return (this.Owner is Model) ? null : this.Owner; }
        }

        [DataMember]
        public ICollection<ModelElement> Members
        {
            get
            {
                if (this.MembersCollection == null)
                    this.MembersCollection = MembersAssociation.GetTargetCollectionFor(this);
                return this.MembersCollection;
            }
            set
            {
                this.Members.Clear();
                foreach (var item in value)
                    this.Members.Add(item);
            }
        }

        /// <summary>
        /// Enumerates this and all nested packages.
        /// </summary>
        public IEnumerable<Package> GetThisAndNestedPackages()
        {
            return this.Find(x => x is Package, false).OfType<Package>();
        }

        /// <summary>
        /// Find subitems matching the given predicate.
        /// </summary>
        public override IEnumerable<ModelBase> Find(Predicate<ModelBase> predicate, bool includeFeatures)
        {
            foreach (var item in base.Find(predicate, includeFeatures))
                yield return item;

            foreach (var item in this.Members)
                foreach (var subitem in item.Find(predicate, includeFeatures))
                    yield return subitem;
        }

        public override void Print(System.IO.TextWriter writer, string prefix)
        {
            base.Print(writer, prefix);
            foreach (var item in this.Members)
                item.Print(writer, prefix + "  ");
        }
    }
}
