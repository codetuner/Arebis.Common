using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.ChangeDataCapture
{
    public abstract class BaseChangeTable
    {
        [Key]
        [Column("__$start_lsn", Order = 0)]
        public virtual byte[] CdcStartLsn { get; set; }

        [Key]
        [Column("__$end_lsn", Order = 1)]
        public virtual byte[] CdcEndLsn { get; set; }

        [Key]
        [Column("__$seqval", Order = 2)]
        public virtual byte[] CdcSequenceValue { get; set; }

        [Key]
        [Column("__$operation", Order = 3)]
        public virtual CdcOperation CdcOperation { get; set; }

        [Column("__$update_mask", Order = 4)]
        public virtual byte[] CdcUpdateMask { get; set; }
    }
}
