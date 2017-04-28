using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [Table("SCHEMATA", Schema = "INFORMATION_SCHEMA")]
    public class Schema : InformationSchemaEntity
    {
        [Key, Column("CATALOG_NAME", Order = 0)]
        public string CatalogName { get; set; }

        [Key, Column("SCHEMA_NAME", Order = 1)]
        public string Name { get; set; }

        [Column("SCHEMA_OWNER")]
        public string SchemaOwner { get; set; }

        [InverseProperty("Schema")]
        public virtual ICollection<Table> Tables { get; set; }

        [InverseProperty("Schema")]
        public virtual ICollection<View> Views { get; set; }

        [InverseProperty("Schema")]
        public virtual ICollection<CheckConstraint> CheckConstraints { get; set; }

        //[InverseProperty("Schema")]
        //public virtual ICollection<ReferentialConstraint> ReferentialConstraints { get; set; }

        [InverseProperty("Schema")]
        public virtual ICollection<Routine> Routines { get; set; }

        public bool Equals(Schema other)
        {
            if (other == null) return false;
            return (this.CatalogName == other.CatalogName)
                && (this.Name == other.Name);
        }
    }
}
