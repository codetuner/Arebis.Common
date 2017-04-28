using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    public class InformationSchemaContext : DbContext
    {
        static InformationSchemaContext()
        {
            Database.SetInitializer<InformationSchemaContext>(null);
        }

        /// <summary>
        /// Creates an InformationSchemaContext for the "DefaultConnection".
        /// </summary>
        public InformationSchemaContext()
            : this("DefaultConnection")
        { }

        public InformationSchemaContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            this.Configuration.AutoDetectChangesEnabled = false;
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += OnObjectMaterialized;
        }

        void OnObjectMaterialized(object sender, System.Data.Entity.Core.Objects.ObjectMaterializedEventArgs e)
        {
            if (e.Entity is InformationSchemaEntity)
            {
                ((InformationSchemaEntity)e.Entity).Context = this;
            }
        }

        public virtual DbSet<Schema> Schemas { get; set; }
        public virtual DbSet<Table> Tables { get; set; }
        public virtual DbSet<TableColumn> TableColumns { get; set; }
        public virtual DbSet<KeyColumn> KeyColumns { get; set; }
        public virtual DbSet<TableConstraint> TableConstraints { get; set; }
        public virtual DbSet<CheckConstraint> CheckConstraints { get; set; }
        public virtual DbSet<ReferentialConstraint> ReferentialConstraints { get; set; }
        public virtual DbSet<ConstraintTable> ConstraintTables { get; set; }
        public virtual DbSet<ConstraintColumn> ConstraintColumns { get; set; }
        public virtual DbSet<View> Views { get; set; }
        public virtual DbSet<ViewColumn> ViewColumns { get; set; }
        public virtual DbSet<ViewTable> ViewTables { get; set; }
        public virtual DbSet<Routine> Routines { get; set; }
        public virtual DbSet<RoutineColumn> RoutineColumns { get; set; }
        public virtual DbSet<Parameter> Parameters { get; set; }

        public sealed override int SaveChanges()
        {
            throw new NotSupportedException("This context is read-only.");
        }

        public sealed override Task<int> SaveChangesAsync()
        {
            throw new NotSupportedException("This context is read-only.");
        }

        public sealed override Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken)
        {
            throw new NotSupportedException("This context is read-only.");
        }

        public Schema GetSchema(string schemaName)
        {
            return this.Schemas
                .OrderBy(s => s.Name)
                .FirstOrDefault(s => s.Name == schemaName);
        }

        public Table GetTable(string schemaName, string tableName)
        {
            return this.Tables
                .OrderBy(t => t.SchemaName).ThenBy(t => t.Name)
                .FirstOrDefault(t => t.SchemaName == schemaName && t.Name == tableName);
        }
    }
}
