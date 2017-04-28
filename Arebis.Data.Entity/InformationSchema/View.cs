using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [Table("VIEWS", Schema = "INFORMATION_SCHEMA")]
    public class View : InformationSchemaEntity
    {
        [Key, ForeignKey("Schema"), Column("TABLE_CATALOG", Order = 0)]
        public string CatalogName { get; set; }

        [Key, ForeignKey("Schema"), Column("TABLE_SCHEMA", Order = 1)]
        public string SchemaName { get; set; }

        public virtual Schema Schema { get; set; }

        [Key, Column("TABLE_NAME", Order = 2)]
        public string Name { get; set; }

        [Column("VIEW_DEFINITION")]
        public string Definition { get; set; }

        [Column("CHECK_OPTION")]
        public string CheckOption { get; set; }
        
        [Column("IS_UPDATABLE")]
        public string IsUpdatableStr { get; set; }

        [InverseProperty("view")]
        public virtual ICollection<ViewColumn> Columns { get; set; }

        [InverseProperty("View")]
        public virtual ICollection<ViewTable> Tables { get; set; }
    }
}
