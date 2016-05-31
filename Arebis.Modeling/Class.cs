using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Modeling
{
    [DataContract(Namespace = "urn:arebis.be:Modeling")]
    public class Class : ModelElement, IModelType
    {
        public static readonly AssociationProperty<Class, Attribute> AttributesAssociation = new AssociationProperty<Class, Attribute>("TypeAttributes", AssociationMultiplicity.Single, AssociationMultiplicity.Multiple);
        private ICollection<Attribute> AttributesCollection = null;

        [DataMember]
        public ICollection<Attribute> Attributes
        {
            get
            {
                if (this.AttributesCollection == null)
                    this.AttributesCollection = AttributesAssociation.GetTargetCollectionFor(this);
                return this.AttributesCollection;
            }
            set
            {
                this.Attributes.Clear();
                foreach (var item in value)
                    this.Attributes.Add(item);
            }
        }

        public static readonly AssociationProperty<Class, Method> MethodsAssociation = new AssociationProperty<Class, Method>("TypeMethods", AssociationMultiplicity.Single, AssociationMultiplicity.Multiple);
        private ICollection<Method> MethodsCollection = null;

        [DataMember]
        public ICollection<Method> Methods
        {
            get
            {
                if (this.MethodsCollection == null)
                    this.MethodsCollection = MethodsAssociation.GetTargetCollectionFor(this);
                return this.MethodsCollection;
            }
            set
            {
                this.Methods.Clear();
                foreach (var item in value)
                    this.Methods.Add(item);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsAbstract { get; set; }

        public bool IsEnumeration
        {
            get { return this.Stereotypes.Contains("enumeration"); }
        }

        public override IEnumerable<ModelBase> Find(Predicate<ModelBase> predicate, bool includeFeatures)
        {
            foreach (var item in base.Find(predicate, includeFeatures))
                yield return item;

            if (includeFeatures)
            {
                foreach (var item in this.Attributes)
                    foreach (var subitem in item.Find(predicate, includeFeatures))
                        yield return subitem;
                foreach (var item in this.Methods)
                    foreach (var subitem in item.Find(predicate, includeFeatures))
                        yield return subitem;
            }
        }

        public override void Print(System.IO.TextWriter writer, string prefix)
        {
            base.Print(writer, prefix);
            foreach (var item in this.Attributes)
                item.Print(writer, prefix + "  ");
            foreach (var item in this.Methods)
                item.Print(writer, prefix + "  ");
        }
    }
}
