using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.ChangeDataCapture
{
    [Table("index_columns", Schema = "cdc")]
    public class IndexColumn : ChangeDataCaptureEntity
    {
        [Key]
        [Column("object_id", Order = 0)]
        [ForeignKey("Table")]
        public int ObjectId { get; set; }

        [Key]
        [Column("index_ordinal", Order = 1)]
        public int IndexOrdinal { get; set; }

        [Key]
        [Column("column_id", Order = 2)]
        public int ColumnId { get; set; }

        [Column("column_name")]
        public string ColumnName { get; set; }

        public virtual ChangeTable Table { get; set; }
    }
}
