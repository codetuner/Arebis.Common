using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [Table("CHECK_CONSTRAINTS", Schema = "INFORMATION_SCHEMA")]
    public class CheckConstraint : InformationSchemaEntity
    {
        [Key, ForeignKey("Schema"), Column("CONSTRAINT_CATALOG", Order = 0)]
        public string ConstraintCatalog { get; set; }

        [Key, ForeignKey("Schema"), Column("CONSTRAINT_SCHEMA", Order = 1)]
        public string ConstraintSchema { get; set; }

        public virtual Schema Schema { get; set; }

        [Key, Column("CONSTRAINT_NAME", Order = 2)]
        public string ConstraintName { get; set; }

        [Column("CHECK_CLAUSE")]
        public string CheckClause { get; set; }

        //[NotMapped]
        //public IQueryable<ConstraintTable> ConstrainedTables
        //{
        //    get
        //    {
        //        return this.Context.ConstraintTables
        //            .Where(ct => ct.ConstraintCatalog == this.ConstraintCatalog && ct.ConstraintSchema == this.ConstraintSchema && ct.ConstraintName == this.ConstraintName);
        //    }
        //}

        //[NotMapped]
        //public IQueryable<ConstraintColumn> ConstrainedColumns
        //{
        //    get
        //    {
        //        return this.Context.ConstraintColumns
        //            .Where(cc => cc.ConstraintCatalog == this.ConstraintCatalog && cc.ConstraintSchema == this.ConstraintSchema && cc.ConstraintName == this.ConstraintName);
        //    }
        //}

        public IQueryable<ConstraintTable> GetConstrainedTables()
        {
            return this.Context.ConstraintTables
                .Where(cc => cc.ConstraintCatalog == this.ConstraintCatalog && cc.ConstraintSchema == this.ConstraintSchema && cc.ConstraintName == this.ConstraintName);
        }

        public IQueryable<ConstraintColumn> GetConstrainedColumns()
        {
            return this.Context.ConstraintColumns
                .Where(cc => cc.ConstraintCatalog == this.ConstraintCatalog && cc.ConstraintSchema == this.ConstraintSchema && cc.ConstraintName == this.ConstraintName);
        }
    }
}
