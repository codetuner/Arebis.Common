using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [Table("PARAMETERS", Schema = "INFORMATION_SCHEMA")]
    public class Parameter : InformationSchemaEntity
    {
        [Key, Column("SPECIFIC_CATALOG", Order = 0)]
        [ForeignKey("Specific")]
        public string SpecificCatalog { get; set; }

        [Key, Column("SPECIFIC_SCHEMA", Order = 1)]
        [ForeignKey("Specific")]
        public string SpecificSchema { get; set; }

        [Key, Column("SPECIFIC_NAME", Order = 2)]
        [ForeignKey("Specific")]
        public string SpecificName { get; set; }

        public virtual Routine Specific { get; set; }

        [Key, Column("ORDINAL_POSITION", Order = 3)]
        public int OrdinalPosition { get; set; }

        [Column("PARAMETER_MODE")]
        public string ParameterMode { get; set; }

        [Column("IS_RESULT")]
        public string IsResultString { get; set; }

        [NotMapped]
        public bool? IsResult
        {
            get
            {
                if (IsResultString == null) return null;
                else if (IsResultString == "YES") return true;
                else return false;
            }
        }

        [Column("AS_LOCATOR")]
        public string AsLocatorString { get; set; }

        [NotMapped]
        public bool? AsLocator
        {
            get
            {
                if (AsLocatorString == null) return null;
                else if (AsLocatorString == "YES") return true;
                else return false;
            }
        }

        [Column("PARAMETER_NAME")]
        public string ParameterName { get; set; }

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

        [Column("INTERVAL_TYPE")]
        public string IntervalType { get; set; }

        [Column("INTERVAL_PRECISION")]
        public short? IntervalPrecision { get; set; }

        public CharacterSet CharacterSet { get; set; }

        public Collation Collation { get; set; }
        
        public UserDefinedType UDT { get; set; }
        
        public Scope Scope { get; set; }
    }
}