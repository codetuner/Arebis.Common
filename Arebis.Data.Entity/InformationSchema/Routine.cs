using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [Table("ROUTINES", Schema = "INFORMATION_SCHEMA")]
    public class Routine : InformationSchemaEntity
    {
        [Key, Column("SPECIFIC_CATALOG", Order = 0)]
        public string SpecificCatalogName { get; set; }

        [Key, Column("SPECIFIC_SCHEMA", Order = 1)]
        public string SpecificSchemaName { get; set; }

        [Key, Column("SPECIFIC_NAME", Order = 2)]
        public string SpecificName { get; set; }

        [Column("ROUTINE_CATALOG", Order = 3)]
        [ForeignKey("Schema")]
        public string CatalogName { get; set; }

        [Column("ROUTINE_SCHEMA", Order = 4)]
        [ForeignKey("Schema")]
        public string SchemaName { get; set; }

        public virtual Schema Schema { get; set; }

        [Column("ROUTINE_NAME", Order = 5)]
        public string Name { get; set; }

        [Column("ROUTINE_TYPE")]
        public string Type { get; set; }

        [Column("MODULE_CATALOG")]
        public string ModuleCatalogName { get; set; }

        [Column("MODULE_SCHEMA")]
        public string ModuleSchemaName { get; set; }

        [Column("MODULE_NAME")]
        public string ModuleName { get; set; }

        [Column("UDT_CATALOG")]
        public string UdtCatalogName { get; set; }

        [Column("UDT_SCHEMA")]
        public string UdtSchemaName { get; set; }

        [Column("UDT_NAME")]
        public string UdtName { get; set; }

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

        [Column("ROUTINE_BODY")]
        public string Body { get; set; }

        [Column("ROUTINE_DEFINITION")]
        public string Definition { get; set; }

        [Column("SQL_DATA_ACCESS")]
        public string SqlDataAccess { get; set; }

        [InverseProperty("Specific")]
        public virtual ICollection<Parameter> Parameters { get; set; }
    }
}
