using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Arebis.Data.Entity
{
    public static class EntityExtensions
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

        public static string GetEntitySetNameOf<TEntity>(this DbContext context)
            where TEntity : class
        {
            return (context as IObjectContextAdapter).ObjectContext.CreateObjectSet<TEntity>().EntitySet.Name;
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
        /// If state Added, returns true. For other states, returns whether the original values
        /// </summary>
        public static bool HasValueChanged(this DbEntityEntry entry, string propertyName)
        {
            if (entry.State == EntityState.Added)
                return true;
            else
                return !Object.Equals(entry.OriginalValues[propertyName], entry.CurrentValues[propertyName]);
        }

        /// <summary>
        /// Whether the given property's value has changed. Also returns the originalValue.
        /// If state Added, returns true and null as the originalValue. For other states, returns whether the original values
        /// have changed compared to the current values.
        /// </summary>
        public static bool HasValueChanged(this DbEntityEntry entry, string propertyName, out object originalValue)
        {
            if (entry.State == EntityState.Added)
            {
                originalValue = null;
                return true;
            }
            else
            {
                originalValue = entry.OriginalValues[propertyName];
                return !Object.Equals(originalValue, entry.CurrentValues[propertyName]);
            }
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

        /// <summary>
        /// Marks this entity as modified.
        /// </summary>
        public static void Update<TContext>(this IContextualEntity<TContext> entity)
            where TContext : DbContext
        {
            if (entity.Context == null)
                throw new InvalidOperationException("No context attached to entity to update.");
            if (entity.Context.Entry(entity).State == EntityState.Deleted)
                throw new InvalidOperationException("Entity to update is already marked deleted.");

            if (entity.Context.Entry(entity).State != EntityState.Added)
            {
                entity.Context.Entry(entity).State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Creates a new instance of an entity for the type of this set or for a type derived
        /// from the type of this set. Note that this instance is NOT added or attached to
        /// the set. The instance returned will be a proxy if the underlying context is configured
        /// to create proxies and the entity type meets the requirements for creating a proxy.
        /// </summary>
        public static TEntity Create<TEntity>(this IDbSet<TEntity> set, Type derivedType)
            where TEntity : class
        {
            var genericCreateMethod = set.GetType().GetMethods().Single(m => m.Name == "Create" && m.ContainsGenericParameters && m.GetParameters().Count() == 0);
            return (TEntity)genericCreateMethod
                .MakeGenericMethod(new Type[] { derivedType })
                .Invoke(set, null);
        }
    }
}
