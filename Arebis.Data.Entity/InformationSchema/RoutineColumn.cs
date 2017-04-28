using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [Table("ROUTINE_COLUMNS", Schema = "INFORMATION_SCHEMA")]
    public class RoutineColumn : InformationSchemaEntity
    {
        [Key, Column("TABLE_CATALOG", Order = 0)]
        public string TableCatalog { get; set; }

        [Key, Column("TABLE_SCHEMA", Order = 1)]
        public string TableSchema { get; set; }

        [Key, Column("TABLE_NAME", Order = 2)]
        public string TableName { get; set; }

        [Key, Column("COLUMN_NAME", Order = 3)]
        public string ColumnName { get; set; }

        [Column("ORDINAL_POSITION")]
        public int OrdinalPosition { get; set; }

        [Column("COLUMN_DEFAULT")]
        public string DefaultValue { get; set; }

        [Column("IS_NULLABLE")]
        public string IsNullableString { get; set; }

        [NotMapped]
        public bool? IsNullable
        {
            get
            {
                if (IsNullableString == null) return null;
                else if (IsNullableString == "YES") return true;
                else return false;
            }
        }

        [Column("DATA_TYPE")]
        public string DataType { get; set; }

        [Column("CHARACTER_MAXIMUM_LENGTH")]
        public int? MaxCharLength { get; set; }

        [Column("CHARACTER_OCTET_LENGTH")]
        public int? MaxByteLength { get; set; }

        [Column("NUMERIC_PRECISION")]
        public byte? NumericPrecision { get; set; }//tinyint

        [Column("NUMERIC_PRECISION_RADIX")]
        public short? NumercPrecisionRadix { get; set; }//smallint

        [Column("NUMERIC_SCALE")]
        public int? NumericScale { get; set; }

        [Column("DATETIME_PRECISION")]
        public short? DateTimePrecision { get; set; }//smallint

        public CharacterSet CharacterSet { get; set; }

        public Collation Collation { get; set; }

        public Domain Domain { get; set; }
    }
}
