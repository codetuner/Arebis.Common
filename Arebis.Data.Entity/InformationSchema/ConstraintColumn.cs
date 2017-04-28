using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [Table("CONSTRAINT_COLUMN_USAGE", Schema = "INFORMATION_SCHEMA")]
    public class ConstraintColumn : InformationSchemaEntity
    {
        [Key, ForeignKey("Column"), Column("TABLE_CATALOG", Order = 0)]
        public string CatalogName { get; set; }

        [Key, ForeignKey("Column"), Column("TABLE_SCHEMA", Order = 1)]
        public string SchemaName { get; set; }

        [Key, ForeignKey("Column"), Column("TABLE_NAME", Order = 2)]
        public string TableName { get; set; }

        [Key, ForeignKey("Column"), Column("COLUMN_NAME", Order = 3)]
        public string ColumnName { get; set; }

        [Key, ForeignKey("Constraint"), Column("CONSTRAINT_CATALOG", Order = 4)]
        public string ConstraintCatalog { get; set; }

        [Key, ForeignKey("Constraint"), Column("CONSTRAINT_SCHEMA", Order = 5)]
        public string ConstraintSchema { get; set; }

        [Key, ForeignKey("Constraint"), Column("CONSTRAINT_NAME", Order = 6)]
        public string ConstraintName { get; set; }
        
        public virtual TableColumn Column { get; set; }

        public virtual TableConstraint Constraint { get; set; }
    }
}
