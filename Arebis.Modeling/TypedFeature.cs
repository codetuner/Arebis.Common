using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Modeling
{
    [DataContract(Namespace = "urn:arebis.be:Modeling")]
    [KnownType(typeof(Attribute))]
    [KnownType(typeof(Method))]
    [KnownType(typeof(AssociationEnd))]
    public abstract class TypedFeature : ModelBase
    {
        [DataMember()]
        public IModelType Type { get; set; }

        public string TypeName
        {
            get
            {
                return (this.Type == null) ? null : this.Type.Name;
            }
        }

        /// <summary>
        /// O if minimal zero, 1 of minimal 1.
        /// </summary>
        public int MultiplicityLowerBound 
        {
            get 
            {
                if ((Multiplicity != null) && (Multiplicity.StartsWith("1")))
                    return 1;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 1 of maximal 1, 2 of maximal n.
        /// </summary>
        public int MultiplicityUpperBound
        {
            get
            {
                if ((Multiplicity != null) && (Multiplicity.EndsWith("1")))
                    return 1;
                else
                    return 2;
            }
        }

        [DataMember]
        public string Multiplicity { get; set; }

        [DataMember()]
        public bool IsDerived { get; set; }

        public override void Print(System.IO.TextWriter writer, string prefix)
        {
            writer.WriteLine(prefix + "{0}: [{3}] {1} : {2}", this.GetType().Name, this, this.Type, this.Multiplicity ?? "*");
        }
    }
}
