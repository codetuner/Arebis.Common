using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [Table("TABLES", Schema = "INFORMATION_SCHEMA")]
    public class Table : InformationSchemaEntity
    {
        [Key, ForeignKey("Schema"), Column("TABLE_CATALOG", Order = 0)]
        public string CatalogName { get; set; }

        [Key, ForeignKey("Schema"), Column("TABLE_SCHEMA", Order = 1)]
        public string SchemaName { get; set; }

        public virtual Schema Schema { get; set; }

        [Key, Column("TABLE_NAME", Order = 2)]
        public string Name { get; set; }

        [Column("TABLE_TYPE")]
        public string Type { get; set; }

        [NotMapped]
        public bool TypeIsBaseTable
        {
            get
            {
                return this.Type == "BASE TABLE";
            }
        }

        [InverseProperty("Table")]
        public virtual ICollection<TableColumn> Columns { get; set; }

        [InverseProperty("Table")]
        public virtual ICollection<KeyColumn> KeyColumns { get; set; }

        [InverseProperty("Table")]
        public virtual ICollection<TableConstraint> Constraints { get; set; }

        public IEnumerable<TableConstraint> GetForeignKeyConstraints()
        {
            return this.Constraints
                .Where(c => c.Type == "FOREIGN KEY");
        }

        public IEnumerable<TableConstraint> GetInverseForeignKeyConstraints()
        {
            return this.Constraints
                .Where(c => c.Type == "PRIMARY KEY")
                .SelectMany(c => c.UniqueConstraintOf)
                .Select(r => r.ForeignConstraint);
        }

        public TableConstraint GetPrimaryKeyConstraint()
        {
            return this.Constraints
                .Where(c => c.Type == "PRIMARY KEY")
                .SingleOrDefault();
        }

        public bool Equals(Table other)
        {
            if (other == null) return false;
            return (this.Schema.Equals(other.Schema))
                && (this.Name == other.Name);
        }

        public bool Matches(ViewTable other)
        {
            if (other == null) return false;
            return (this.Equals(other.Table));
        }

        public bool Matches(ConstraintTable other)
        {
            if (other == null) return false;
            return (this.Equals(other.Table));
        }
    }
}
