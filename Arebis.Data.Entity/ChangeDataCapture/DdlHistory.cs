using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.ChangeDataCapture
{
    [Table("ddl_history", Schema = "cdc")]
    public class DdlHistory : ChangeDataCaptureEntity
    {
        [Key]
        [Column("object_id", Order = 0)]
        [ForeignKey("Table")]
        public int ObjectId { get; set; }

        [Key]
        [Column("ddl_lsn", Order = 1)]
        public byte[] Lsn { get; set; }

        [Column("source_object_id")]
        public int? SourceObjectId { get; set; }

        [Column("required_column_update")]
        public bool? RequiredColumnUpdate { get; set; }

        [Column("ddl_command")]
        public string Command { get; set; }

        [Column("ddl_time")]
        public DateTime? DateTime { get; set; }

        public virtual ChangeTable Table { get; set; }
    }
}
