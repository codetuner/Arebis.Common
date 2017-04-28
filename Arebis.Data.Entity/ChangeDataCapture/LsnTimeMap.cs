using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.ChangeDataCapture
{
    [Table("lsn_time_mapping", Schema = "cdc")]
    public class LsnTimeMap : ChangeDataCaptureEntity
    {
        [Key]
        [Column("start_lsn")]
        public byte[] StartLsn { get; set; }

        [Column("tran_begin_time")]
        public DateTime? TransactionBeginTime { get; set; }

        [Column("tran_end_time")]
        public DateTime? TransactionEndTime { get; set; }

        [Column("tran_id")]
        public byte[] TransactionId { get; set; }

        [Column("tran_begin_lsn")]
        public byte[] TransactionBeginLsn { get; set; }
    }
}
