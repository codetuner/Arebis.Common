using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Modeling
{
    public enum AssociationMultiplicity
    {
        Multiple = 0,
        Single = 1
    }

    public class AssociationProperty<TSource, TTarget>
    {
        private string name;
        private List<Tuple<TSource, TTarget>> associations = new List<Tuple<TSource, TTarget>>();

        public AssociationProperty(string name)
            : this(name, AssociationMultiplicity.Multiple, AssociationMultiplicity.Multiple)
        { }

        public AssociationProperty(string name, AssociationMultiplicity sourceMultiplicity, AssociationMultiplicity targetMultiplicity)
        {
            this.name = name;
            this.SourceMultiplicity = sourceMultiplicity;
            this.TargetMultiplicity = targetMultiplicity;
        }

        public AssociationMultiplicity SourceMultiplicity { get; set; }

        public AssociationMultiplicity TargetMultiplicity { get; set; }

        public IEnumerable<TSource> GetSourcesFor(TTarget target)
        {
            return this.associations.Where(a => a.Item2.Equals(target)).Select(a => a.Item1);
        }

        public IEnumerable<TTarget> GetTargetsFor(TSource source)
        {
            return this.associations.Where(a => a.Item1.Equals(source)).Select(a => a.Item2);
        }

        public bool SetOrAdd(TSource source, TTarget target)
        {
            #region Arguments check
            if (source == null && target == null)
                throw new ArgumentNullException("Arguments source and target cannot be both null.", (Exception)null);
            #endregion

            if (SourceMultiplicity == AssociationMultiplicity.Single)
            {
                if (TargetMultiplicity == AssociationMultiplicity.Single)
                {
                    if (source == null)
                    {
                        return (this.associations.RemoveAll(a => a.Item2.Equals(target)) > 0);
                    }
                    else if (target == null)
                    {
                        return (this.associations.RemoveAll(a => a.Item1.Equals(source)) > 0);
                    }
                    else // if both are non-null:
                    {
                        this.associations.RemoveAll(a => a.Item1.Equals(source) || a.Item2.Equals(target));
                        this.associations.Add(new Tuple<TSource, TTarget>(source, target));
                        return true;
                    }
                }
                else
                {
                    if (target != null)
                    {
                        this.associations.RemoveAll(a => a.Item2.Equals(target));
                        if (source != null)
                            this.associations.Add(new Tuple<TSource, TTarget>(source, target));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (TargetMultiplicity == AssociationMultiplicity.Single)
                {
                    if (source != null)
                    {
                        this.associations.RemoveAll(a => a.Item1.Equals(source));
                        if (target != null)
                            this.associations.Add(new Tuple<TSource, TTarget>(source, target));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (source != null && target != null)
                    {
                        this.associations.RemoveAll(a => a.Item1.Equals(source) && a.Item2.Equals(target));
                        this.associations.Add(new Tuple<TSource, TTarget>(source, target));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public bool Remove(TSource source, TTarget target)
        {
            #region Arguments check
            if (source == null)
                throw new ArgumentNullException("source", "Arguments source cannot be null.");
            if (target == null)
                throw new ArgumentNullException("target", "Arguments target cannot be null.");
            #endregion

            return (this.associations.RemoveAll(a => a.Item1.Equals(source) && a.Item2.Equals(target)) > 0);
        }

        public ICollection<TSource> GetSourceCollectionFor(TTarget target)
        {
            return new AssociationSourceCollection(this, target);
        }

        public ICollection<TTarget> GetTargetCollectionFor(TSource source)
        {
            return new AssociationTargetCollection(this, source);
        }

        #region Inner collection classes

        [Serializable /* To please DataContractSerializer */]
        internal class AssociationSourceCollection : ICollection<TSource>, ISerializable
        {
            private AssociationProperty<TSource, TTarget> property;
            private TTarget target;

            internal AssociationSourceCollection(AssociationProperty<TSource, TTarget> property, TTarget target)
            {
                this.property = property;
                this.target = target;
            }

            public void Add(TSource item)
            {
                this.property.SetOrAdd(item, this.target);
            }

            public void Clear()
            {
                this.property.associations.RemoveAll(t => t.Item2.Equals(target));
            }

            public bool Contains(TSource item)
            {
                return this.property.associations.Where(t => t.Item2.Equals(target)).Select(t => t.Item1).Contains(item);
            }

            public void CopyTo(TSource[] array, int arrayIndex)
            {
                foreach (var item in this)
                {
                    array[arrayIndex] = item;
                    arrayIndex++;
                }
            }

            public int Count
            {
                get { return this.property.associations.Where(t => t.Item2.Equals(target)).Select(t => t.Item1).Count(); }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(TSource item)
            {
                return this.property.Remove(item, this.target);
            }

            public IEnumerator<TSource> GetEnumerator()
            {
                return this.property.associations.Where(t => t.Item2.Equals(target)).Select(t => t.Item1).ToList().GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return (System.Collections.IEnumerator)this.GetEnumerator();
            }

            #region ISerializable Members

            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                throw new NotSupportedException("This serializer is not supported by AssociationProperties; please use the DataContractSerializer.");
            }

            #endregion
        }

        [Serializable /* To please DataContractSerializer */]
        internal class AssociationTargetCollection : ICollection<TTarget>, ISerializable
        {
            private AssociationProperty<TSource, TTarget> property;
            private TSource source;

            internal AssociationTargetCollection(AssociationProperty<TSource, TTarget> property, TSource source)
            {
                this.property = property;
                this.source = source;
            }

            public void Add(TTarget item)
            {
                this.property.SetOrAdd(this.source, item);
            }

            public void Clear()
            {
                this.property.associations.RemoveAll(t => t.Item1.Equals(source));
            }

            public bool Contains(TTarget item)
            {
                return this.property.associations.Where(t => t.Item1.Equals(source)).Select(t => t.Item2).Contains(item);
            }

            public void CopyTo(TTarget[] array, int arrayIndex)
            {
                foreach (var item in this)
                {
                    array[arrayIndex] = item;
                    arrayIndex++;
                }
            }

            public int Count
            {
                get { return this.property.associations.Where(t => t.Item1.Equals(source)).Select(t => t.Item2).Count(); }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(TTarget item)
            {
                return this.property.Remove(this.source, item);
            }

            public IEnumerator<TTarget> GetEnumerator()
            {
                return this.property.associations.Where(t => t.Item1.Equals(source)).Select(t => t.Item2).ToList().GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return (System.Collections.IEnumerator)this.GetEnumerator();
            }

            #region ISerializable Members

            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                throw new NotSupportedException("This serializer is not supported by AssociationProperties; please use the DataContractSerializer.");
            }

            #endregion
        }

        #endregion
    }
}
