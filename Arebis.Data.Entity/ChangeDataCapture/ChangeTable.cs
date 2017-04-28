using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.ChangeDataCapture
{
    [Table("change_tables", Schema = "cdc")]
    public class ChangeTable : ChangeDataCaptureEntity
    {
        [Key]
        [Column("object_id")]
        public int ObjectId { get; set; }

        [Column("version")]
        public int? Version { get; set; }

        [Column("source_object_id")]
        public int? SourceObjectId { get; set; }

        [Column("capture_instance")]
        public string CaptureInstance { get; set; }

        [Column("start_lsn")]
        public byte[] StartLsn { get; set; }

        [Column("end_lsn")]
        public byte[] EndLsn { get; set; }

        [Column("supports_net_changes")]
        public bool? SupportsNetChanges { get; set; }

        [Column("has_drop_pending")]
        public bool? HasDropPending { get; set; }

        [Column("role_name")]
        public string RoleName { get; set; }

        [Column("index_name")]
        public string IndexName { get; set; }

        [Column("filegroup_name")]
        public string FilegroupName { get; set; }

        [Column("create_date")]
        public DateTime? CreationDateTime { get; set; }

        [Column("partition_switch")]
        public bool PartitionSwitch { get; set; }

        [InverseProperty("Table")]
        public virtual ICollection<CapturedColumn> CapturedColumns { get; set; }

        [InverseProperty("Table")]
        public virtual ICollection<IndexColumn> IndexColumns { get; set; }

        [InverseProperty("Table")]
        public virtual ICollection<DdlHistory> DdlHistory { get; set; }

        public QueryMapper GetRecords(byte[] afterStartLsn, byte[] uptoStartLsn, CdcOperation[] operations = null, string additionalWhereCondition = null)
        {
            var sb = new StringBuilder();
            sb.Append("SELECT [__$start_lsn] AS [_StartLsn], [__$end_lsn] AS [_EndLsn], [__$seqval] AS [_SequenceValue], [__$operation] AS [_Operation], [__$update_mask] AS [_UpdateMask], [");
            sb.Append(String.Join("], [", CapturedColumns.OrderBy(c => c.ColumnOrdinal).Select(c => c.ColumnName)));
            sb.Append("] FROM ");
            sb.Append("[cdc].[");
            sb.Append(this.CaptureInstance);
            sb.Append("_CT] WHERE ([__$start_lsn] > 0x");
            sb.Append(BitConverter.ToString(afterStartLsn ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }).Replace("-", ""));
            sb.Append(") AND ([__$start_lsn] <= 0x");
            sb.Append(BitConverter.ToString(uptoStartLsn ?? new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }).Replace("-", ""));
            sb.Append(")");
            if (operations != null && operations.Length > 0)
            {
                sb.Append(" AND ([__$operation] IN (");
                sb.Append(String.Join(",", operations.Distinct().Select(o => ((int)o).ToString())));
                sb.Append(")");
            }
            if (!String.IsNullOrWhiteSpace(additionalWhereCondition))
            {
                sb.Append(" AND (");
                sb.Append(additionalWhereCondition);
                sb.Append(")");
            }
            sb.Append(" ORDER BY [__$start_lsn], [__$seqval], [__$operation]");

            return new QueryMapper(this.Context.Database.Connection, sb.ToString());
        }
    }
}
