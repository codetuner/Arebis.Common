using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Arebis.Collections.Generic;

namespace Arebis.Modeling
{
    /// <summary>
    /// Base class for model classes.
    /// </summary>
    [DataContract(IsReference = true, Namespace = "urn:arebis.be:Modeling")]
    [KnownType(typeof(Model))]
    [KnownType(typeof(ModelElement))]
    [KnownType(typeof(TypedFeature))]
    public abstract class ModelBase
    {
        public ModelBase()
        {
            this.Stereotypes = new List<string>();
            this.TaggedValues = new DefaultDictionary<string, string>();
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Name { get; set; }

        public abstract string FullName { get; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<string> Stereotypes { get; private set; }

        public string Documentation
        {
            get
            {
                string result;
                if (this.TaggedValues.TryGetValue("documentation", out result))
                    return result;
                else
                    return null;
            }
            set
            {
                this.TaggedValues["documentation"] = value;
            }
        }

        private ICollection<Relationship> SourceOfCollection = null;

        [DataMember()]
        public ICollection<Relationship> SourceOf
        {
            get
            {
                if (this.SourceOfCollection == null)
                    this.SourceOfCollection = Relationship.SourcesAssociation.GetSourceCollectionFor(this);
                return this.SourceOfCollection;
            }
            set
            {
                this.SourceOf.Clear();
                foreach (var item in value)
                    this.SourceOf.Add(item);
            }
        }

        private ICollection<Relationship> TargetOfCollection = null;

        [DataMember()]
        public ICollection<Relationship> TargetOf
        {
            get
            {
                if (this.TargetOfCollection == null)
                    this.TargetOfCollection = Relationship.TargetsAssociation.GetSourceCollectionFor(this);
                return this.TargetOfCollection;
            }
            set
            {
                this.TargetOf.Clear();
                foreach (var item in value)
                    this.TargetOf.Add(item);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IDictionary<string, string> TaggedValues { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Visibility { get; set; }

        public bool IsPublic
        {
            get { return "public".Equals(this.Visibility, StringComparison.CurrentCultureIgnoreCase); }
        }

        /// <summary>
        /// Find subitems matching the given predicate.
        /// </summary>
        public virtual IEnumerable<ModelBase> Find(System.Predicate<ModelBase> predicate, bool includeFeatures)
        {
            if (predicate(this))
                yield return this;
        }

        /// <summary>
        /// Lists the direct and indirect targets in of type T and given relationship type.
        /// </summary>
        public ICollection<T> FindTransientRelationshipTargetsOfType<T>(string relationshipType)
            where T : ModelBase
        {
            // To avoid circularity, we use a list instead of a stack:
            var list = new List<T>();
            foreach (var target in this.SourceOf.SelectMany(r => r.Targets.OfType<T>()))
                list.Add(target);

            int cursor = 0;
            while (cursor < list.Count)
            {
                var item = list[cursor++];
                foreach (var target in item.SourceOf.SelectMany(r => r.Targets.OfType<T>()))
                    if (!list.Contains(target)) list.Add(target);
            }

            return list;
        }

        /// <summary>
        /// Lists the direct and indirect sources in of type T and given relationship type.
        /// </summary>
        public ICollection<T> FindTransientRelationshipSourcesOfType<T>(string relationshipType)
            where T : ModelBase
        {
            // To avoid circularity, we use a list instead of a stack:
            var list = new List<T>();
            foreach (var target in this.SourceOf.SelectMany(r => r.Sources.OfType<T>()))
                list.Add(target);

            int cursor = 0;
            while (cursor < list.Count)
            {
                var item = list[cursor++];
                foreach (var target in item.SourceOf.SelectMany(r => r.Sources.OfType<T>()))
                    if (!list.Contains(target)) list.Add(target);
            }

            return list;
        }

        public string TaggedValueOr(string key, string defaultValue)
        {
            string value;
            if (this.TaggedValues.TryGetValue(key, out value))
                return value;
            else
                return defaultValue;
        }

        public override string ToString()
        {
            return (String.IsNullOrEmpty(this.Name)) ? "(no name)" : this.Name;
        }

        public virtual void Print(TextWriter writer, string prefix)
        {
            writer.WriteLine(prefix + "{0}: {1}", this.GetType().Name, this);
        }
    }
}
