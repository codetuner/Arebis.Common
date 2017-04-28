using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.ChangeDataCapture
{
    [Table("captured_columns", Schema = "cdc")]
    public class CapturedColumn : ChangeDataCaptureEntity
    {
        [Key]
        [Column("object_id", Order = 0)]
        [ForeignKey("Table")]
        public int ObjectId { get; set; }

        [Key]
        [Column("column_ordinal", Order = 1)]
        public int ColumnOrdinal { get; set; }

        [Column("column_id")]
        public int? ColumnId { get; set; }

        [Column("column_name")]
        public string ColumnName { get; set; }

        [Column("column_type")]
        public string ColumnType { get; set; }

        [Column("is_computed")]
        public bool? IsComputed { get; set; }

        public virtual ChangeTable Table { get; set; }
    }
}
