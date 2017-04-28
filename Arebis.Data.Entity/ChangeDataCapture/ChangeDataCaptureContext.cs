using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.ChangeDataCapture
{
    public class ChangeDataCaptureContext : DbContext
    {
        static ChangeDataCaptureContext()
        {
            Database.SetInitializer<ChangeDataCaptureContext>(null);
        }

        /// <summary>
        /// Creates an ChangeDataCaptureContext for the "DefaultConnection".
        /// </summary>
        public ChangeDataCaptureContext()
            : this("DefaultConnection")
        { }

        public ChangeDataCaptureContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            this.Configuration.AutoDetectChangesEnabled = false;
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += OnObjectMaterialized;
        }

        void OnObjectMaterialized(object sender, System.Data.Entity.Core.Objects.ObjectMaterializedEventArgs e)
        {
            if (e.Entity is ChangeDataCaptureEntity)
            {
                ((ChangeDataCaptureEntity)e.Entity).Context = this;
            }
        }

        public virtual DbSet<ChangeTable> ChangeTables { get; set; }
        public virtual DbSet<CapturedColumn> CapturedColumns { get; set; }
        public virtual DbSet<DdlHistory> DdlHistory { get; set; }
        public virtual DbSet<IndexColumn> IndexColumns { get; set; }
        public virtual DbSet<LsnTimeMap> LsnTimeMapping { get; set; }

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
    }
}
