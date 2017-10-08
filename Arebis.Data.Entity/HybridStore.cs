using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity
{
    /// <summary>
    /// Core of the hybrid store implementation. DbContexts using hybrid storage
    /// must be registered to this hybrid store.
    /// </summary>
    public static class HybridStore
    {
        /// <summary>
        /// Adds/registeres a DbContext with the hybrid storage.
        /// </summary>
        public static void Add(DbContext context)
        {
            var oc = ((IObjectContextAdapter)context).ObjectContext;
            oc.ObjectMaterialized += OnObjectMaterialized;
            oc.SavingChanges += OnSavingChanges;
        }

        /// <summary>
        /// Removes/unregisters a DbContext from the hybrid storage.
        /// This is only needed if you want to continue using the context
        /// but disable the hybrid storage. Registered contexts can safely be
        /// disposed without removing them first.
        /// </summary>
        public static void Remove(DbContext context)
        {
            var oc = ((IObjectContextAdapter)context).ObjectContext;
            oc.ObjectMaterialized -= OnObjectMaterialized;
            oc.SavingChanges -= OnSavingChanges;
        }

        private static void OnObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var doc = e.Entity as IHybridDocument;
            if (doc != null && doc.DocumentData != null)
            {
                JsonConvert.PopulateObject(doc.DocumentData, doc);
            }
        }

        private static void OnSavingChanges(object sender, EventArgs e)
        {
            var context = new DbContext(sender as ObjectContext, false);
            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    var doc = entry.Entity as IHybridDocument;
                    if (doc != null)
                    {
                        var newDocData = JsonConvert.SerializeObject(doc);
                        if (!newDocData.Equals(doc.DocumentData)) doc.DocumentData = newDocData;
                    }
                }
            }
        }
    }
}