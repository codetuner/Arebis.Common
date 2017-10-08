using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
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

        #region Constructors

        public BaseDbContext()
            : base()
        {
            this.Initialize();
        }

        public BaseDbContext(DbCompiledModel model)
            : base(model)
        {
            this.Initialize();
        }

        public BaseDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            this.Initialize();
        }

        public BaseDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            this.Initialize();
        }

        public BaseDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            this.Initialize();
        }

        public BaseDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            this.Initialize();
        }

        public BaseDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            this.Initialize();
        }

        /// <summary>
        /// Invoked by the constructor to initialize.
        /// Override and extend to add initialization logic.
        /// </summary>
        protected virtual void Initialize()
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += OnObjectMaterialized;
            ((IObjectContextAdapter)this).ObjectContext.SavingChanges += OnSavingChanges;
        }

        #endregion

        protected virtual void OnObjectMaterialized(object sender, System.Data.Entity.Core.Objects.ObjectMaterializedEventArgs e)
        {
            // When materialized, set it's context:
            var entity = e.Entity as IContextualEntity<TContext>;
            if (entity != null)
            {
                entity.Context = (TContext)(object)this;
            }

            // Run OnMaterialized:
            var materialized = e.Entity as IMaterializeInterceptable;
            if (materialized != null)
            {
                materialized.OnMaterialized(this);
            }
        }

        protected virtual void OnSavingChanges(object sender, EventArgs e)
        {
            // When saving context, call OnSaving of all entities:
            var tcontext = (TContext)(object)this;
            foreach (var entry in this.ChangeTracker.Entries())
            {
                var entity = entry.Entity as IContextualEntity<TContext>;
                if (entity != null)
                {
                    entity.OnSaving(tcontext, entry);
                }
            }
        }
    }
}
