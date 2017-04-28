using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [Table("TABLE_CONSTRAINTS", Schema = "INFORMATION_SCHEMA")]
    public class TableConstraint : InformationSchemaEntity
    {
        [Key, ForeignKey("Schema"), Column("CONSTRAINT_CATALOG", Order = 0)]
        public string ConstraintCatalog { get; set; }

        [Key, ForeignKey("Schema"), Column("CONSTRAINT_SCHEMA", Order = 1)]
        public string ConstraintSchema { get; set; }

        [Key, Column("CONSTRAINT_NAME", Order = 2)]
        public string ConstraintName { get; set; }

        public virtual Schema Schema { get; set; }

        [ForeignKey("Table"), Column("TABLE_CATALOG", Order = 3)]
        public string TableCatalogName { get; set; }

        [ForeignKey("Table"), Column("TABLE_SCHEMA", Order = 4)]
        public string TableSchemaName { get; set; }

        [ForeignKey("Table"), Column("TABLE_NAME", Order = 5)]
        public string TableName { get; set; }

        public virtual Table Table { get; set; }

        [Column("CONSTRAINT_TYPE")]
        public string Type { get; set; }

        [Column("IS_DEFERRABLE")]
        public string IsDeferrable { get; set; }

        [Column("INITIALLY_DEFERRED")]
        public string InitiallyDeferred { get; set; }

        [InverseProperty("UniqueConstraint")]
        public virtual ICollection<ReferentialConstraint> UniqueConstraintOf { get; set; }

        [InverseProperty("ForeignConstraint")]
        public virtual ReferentialConstraint ForeignConstraintOf { get; set; }

        [InverseProperty("Constraint")]
        public virtual ICollection<ConstraintColumn> ConstrainedColumns { get; set; }

        [InverseProperty("Constraint")]
        public virtual ICollection<ConstraintTable> ConstrainedTables { get; set; }

        //public IQueryable<ConstraintTable> GetConstrainedTables()
        //{
        //    return this.Context.ConstraintTables
        //        .Where(cc => cc.ConstraintCatalog == this.ConstraintCatalog && cc.ConstraintSchema == this.ConstraintSchema && cc.ConstraintName == this.ConstraintName);
        //}

        //public IQueryable<ConstraintColumn> GetConstrainedColumns()
        //{
        //    return this.Context.ConstraintColumns
        //        .Where(cc => cc.ConstraintCatalog == this.ConstraintCatalog && cc.ConstraintSchema == this.ConstraintSchema && cc.ConstraintName == this.ConstraintName);
        //}
    }
}
