using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Model
{
    [Serializable]
    public abstract class DatabaseOwnedElement : BaseModelElement
    {
        public virtual DatabaseModel Model { get; set; }

        public virtual string Schema { get; set; }

        public string SchemaDotName 
        {
            get { return (this.Schema == null) ? this.Name : this.Schema + "." + this.Name; }
        }

        public override string ToString()
        {
            return String.Format("[{0}].[{1}]", this.Schema, this.Name);
        }

        public override bool Equals(object obj)
        {
            var other = obj as DatabaseOwnedElement;
            if (other == null) return false;
            return this.SchemaDotName.Equals(other.SchemaDotName);
        }

        public override int GetHashCode()
        {
            return this.GetType().GetHashCode() ^ this.SchemaDotName.GetHashCode();
        }
    }
}
