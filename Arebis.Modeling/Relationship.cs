using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Modeling
{
    [DataContract(Namespace = "urn:arebis.be:Modeling")]
    public class Relationship : ModelElement
    {
        [DataMember]
        public string Type { get; set; }

        public static readonly AssociationProperty<Relationship, ModelBase> SourcesAssociation = new AssociationProperty<Relationship, ModelBase>("RelationshipSources", AssociationMultiplicity.Multiple, AssociationMultiplicity.Multiple);
        private ICollection<ModelBase> SourcesCollection = null;

        [DataMember]
        public ICollection<ModelBase> Sources
        {
            get
            {
                if (this.SourcesCollection == null)
                    this.SourcesCollection = SourcesAssociation.GetTargetCollectionFor(this);
                return this.SourcesCollection;
            }
            set
            {
                this.Sources.Clear();
                foreach (var item in value)
                    this.Sources.Add(item);
            }
        }

        public static readonly AssociationProperty<Relationship, ModelBase> TargetsAssociation = new AssociationProperty<Relationship, ModelBase>("RelationshipTargets", AssociationMultiplicity.Multiple, AssociationMultiplicity.Multiple);
        private ICollection<ModelBase> TargetsCollection = null;

        [DataMember]
        public ICollection<ModelBase> Targets
        {
            get
            {
                if (this.TargetsCollection == null)
                    this.TargetsCollection = TargetsAssociation.GetTargetCollectionFor(this);
                return this.TargetsCollection;
            }
            set
            {
                this.Targets.Clear();
                foreach (var item in value)
                    this.Targets.Add(item);
            }
        }
    }
}
