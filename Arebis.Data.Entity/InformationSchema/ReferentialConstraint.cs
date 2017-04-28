using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [Table("REFERENTIAL_CONSTRAINTS", Schema = "INFORMATION_SCHEMA")]
    public class ReferentialConstraint : InformationSchemaEntity
    {
        [Key, ForeignKey("ForeignConstraint"), /*ForeignKey("Schema"),*/ Column("CONSTRAINT_CATALOG", Order = 0)]
        public string ForeignConstraintCatalog { get; set; }

        [Key, ForeignKey("ForeignConstraint"), /*ForeignKey("Schema"),*/ Column("CONSTRAINT_SCHEMA", Order = 1)]
        public string ForeignConstraintSchema { get; set; }

        //public virtual Schema Schema { get; set; }

        [Key, ForeignKey("ForeignConstraint"), Column("CONSTRAINT_NAME", Order = 2)]
        public string ForeignConstraintName { get; set; }

        public virtual TableConstraint ForeignConstraint { get; set; }

        [ForeignKey("UniqueConstraint"), Column("UNIQUE_CONSTRAINT_CATALOG", Order = 3)]
        public string UniqueConstraintCatalog { get; set; }

        [ForeignKey("UniqueConstraint"), Column("UNIQUE_CONSTRAINT_SCHEMA", Order = 4)]
        public string UniqueConstraintSchema { get; set; }

        [ForeignKey("UniqueConstraint"), Column("UNIQUE_CONSTRAINT_NAME", Order = 5)]
        public string UniqueConstraintName { get; set; }

        [Column("MATCH_OPTION")]
        public string MatchOption { get; set; }

        [Column("UPDATE_RULE")]
        public string UpdateRule { get; set; }

        [Column("DELETE_RULE")]
        public string DeleteRule { get; set; }

        public virtual TableConstraint UniqueConstraint { get; set; }

        //[NotMapped, Obsolete]
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

        //[NotMapped]
        //public Reference Reference
        //{
        //    get
        //    {
        //        return new Reference()
        //        {
        //            Constraint = this,
        //            PrimaryTable = this.UniqueConstraint.Table,
        //            PrimaryColumns = this.UniqueConstraint.ConstrainedColumns.ToList(),
        //            ForeignTable = this.ConstrainedTables.First(),
        //            ForeignColumns = this.ConstrainedColumns.ToList()
        //        };
        //    }
        //}

        public IQueryable<ConstraintTable> GetConstrainedTables()
        {
            return this.Context.ConstraintTables
                .Where(cc => cc.ConstraintCatalog == this.ForeignConstraintCatalog && cc.ConstraintSchema == this.ForeignConstraintSchema && cc.ConstraintName == this.ForeignConstraintName);
        }

        public IQueryable<ConstraintColumn> GetConstrainedColumns()
        {
            return this.Context.ConstraintColumns
                .Where(cc => cc.ConstraintCatalog == this.ForeignConstraintCatalog && cc.ConstraintSchema == this.ForeignConstraintSchema && cc.ConstraintName == this.ForeignConstraintName);
        }
    }
}
