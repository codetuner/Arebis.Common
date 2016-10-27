using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity
{
    partial class EntityExtensions
    {
        /// <summary>
        /// Returns an array with all primary key values for the given entry's entity.
        /// </summary>
        public static object[] GetPrimaryKeyValue(this DbContext context, DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            var entityKeyValues = objectStateEntry.EntityKey.EntityKeyValues;

            var result = new object[entityKeyValues.Length];
            for (int i = 0; i < entityKeyValues.Length; i++)
            {
                result[i] = entityKeyValues[i].Value;
            }

            return result;
        }

        /// <summary>
        /// Get the name of the EntitySet.
        /// </summary>
        public static string GetEntitySetName(this DbContext context, DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            return objectStateEntry.EntitySet.Name;
        }

        /// <summary>
        /// Get a string array with he name of the database schema and the database table for the given entity entry.
        /// </summary>
        public static string[] GetEntityTableSchemaAndName(this DbContext context, DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            return new string[] { objectStateEntry.EntitySet.Schema, objectStateEntry.EntitySet.Table };
        }

        /// <summary>
        /// Whether the given property's value has changed.
        /// </summary>
        public static bool HasValueChanged(this DbEntityEntry entry, string propertyName)
        {
            if (entry.State == EntityState.Added)
                return true;
            else
                return !Object.Equals(entry.OriginalValues[propertyName], entry.CurrentValues[propertyName]);
        }

        /// <summary>
        /// Returns a dictionary with scalar values changed since last Db interaction.
        /// The dictionary contains pairs with original and current value.
        /// Note that if the entity is in Added, Deleted or Detached state, null is returned.
        /// </summary>
        public static IDictionary<string, object[]> GetChangedValues(this DbEntityEntry entry)
        {
            if (entry.State == EntityState.Modified || entry.State == EntityState.Unchanged)
            {
                var dict = new Dictionary<string, object[]>();

                if (entry.State == EntityState.Modified)
                {
                    foreach (var property in entry.OriginalValues.PropertyNames)
                    {
                        var original = entry.OriginalValues[property];
                        var current = entry.CurrentValues[property];
                        if (!Object.Equals(original, current))
                        {
                            dict[property] = new Object[] { original, current };
                        }
                    }
                }

                return dict;
            }
            else
            {
                return null;
            }
        }
    }
}
