using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity
{
    /// <summary>
    /// Base class for EF6 DbContexts.
    /// </summary>
    public abstract class BaseDbContext<TContext> : DbContext
        where TContext : BaseDbContext<TContext>
    {
        #region Static constructor

        static BaseDbContext()
        {
            string typeName = null;
            try
            {
                typeName = typeof(TContext).Name;
                if (Boolean.Parse(ConfigurationManager.AppSettings[typeName + ".EnableMigrations"] ?? "false") != true)
                {
                    // Disable database migrations:
                    Database.SetInitializer<TContext>(null);
                }
            }
            catch
            {
                System.Diagnostics.Trace.Write("Error parsing " + (typeName ?? "{ContextType}") + ".EnableMigrations. Must be true or false. Migrations disabled by default.");

                // Disable database migrations:
                Database.SetInitializer<TContext>(null);
            }
        }

        #endregion

        /// <summary>
        /// Creates a new DbContext using a connection named "DefaultConnection".
        /// </summary>
        public BaseDbContext()
            : this("DefaultConnection")
        { }

        /// <summary>
        /// Creates a new DbContext given a connection name or connectionstring.
        /// </summary>
        public BaseDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += OnObjectMaterialized;
            ((IObjectContextAdapter)this).ObjectContext.SavingChanges += OnSavingChanges;
        }

        protected virtual void OnObjectMaterialized(object sender, System.Data.Entity.Core.Objects.ObjectMaterializedEventArgs e)
        {
            // Set Context property:
            var contextEntity = e.Entity as Entity<TContext>;
            if (contextEntity != null)
            {
                contextEntity.Context = (TContext)(object)this;
            }

            // Run OnMaterialized:
            var materialized = e.Entity as IMaterializeInterceptable;
            if (contextEntity != null)
            {
                materialized.OnMaterialized(this);
            }
        }

        protected virtual void OnSavingChanges(object sender, EventArgs e)
        {
            foreach (var entry in this.ChangeTracker.Entries())
            {
                var savedEntity = entry.Entity as ISaveInterceptable;
                if (savedEntity != null)
                {
                    savedEntity.OnSaving(this, entry);
                }
            }
        }

        /// <summary>
        /// Marks the entity for deletion.
        /// </summary>
        /// <remarks>
        /// Alternatively, call the Remove method of the entity's DbSet.
        /// </remarks>
        public virtual void DeleteObject(object entity)
        {
            this.Entry(entity).State = EntityState.Deleted;
        }
    }
}
