using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [Table("VIEW_COLUMN_USAGE", Schema = "INFORMATION_SCHEMA")]
    public class ViewColumn : InformationSchemaEntity
    {
        [Key, ForeignKey("View"), Column("VIEW_CATALOG", Order = 0)]
        public string ViewCatalog { get; set; }

        [Key, ForeignKey("View"), Column("VIEW_SCHEMA", Order = 1)]
        public string ViewSchema { get; set; }

        [Key, ForeignKey("View"), Column("VIEW_NAME", Order = 2)]
        public string ViewName { get; set; }

        [Key, ForeignKey("Column"), Column("TABLE_CATALOG", Order = 3)]
        public string CatalogName { get; set; }

        [Key, ForeignKey("Column"), Column("TABLE_SCHEMA", Order = 4)]
        public string SchemaName { get; set; }

        [Key, ForeignKey("Column"), Column("TABLE_NAME", Order = 5)]
        public string TableName { get; set; }

        [Key, ForeignKey("Column"), Column("COLUMN_NAME", Order = 6)]
        public string ColumnName { get; set; }

        public virtual View View { get; set; }
        
        public virtual TableColumn Column { get; set; }
    }
}
