using System;
using System.Data.Entity;

namespace Arebis.Data.Entity
{
    public static class HybridStoreExtensions
    {
        /// <summary>
        /// Call from the constructor method of a DbContext subclass to tell it to
        /// use hybrid storage.
        /// </summary>
        public static TContext UseHybridStorage<TContext>(this TContext context)
            where TContext : DbContext
        {
            HybridStore.Add(context);
            return context;
        }

        /// <summary>
        /// 'Touches' an entity, marking it as modified.
        /// </summary>
        public static void Update(this DbContext context, object entity)
        {
            if (context.Entry(entity).State == EntityState.Deleted)
            {
                throw new InvalidOperationException("Cannot 'touch' an entity that is marked deleted.");
            }
            else if (context.Entry(entity).State != EntityState.Added)
            {
                context.Entry(entity).State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Marks an entity as to be deleted.
        /// </summary>
        public static void Remove(this DbContext context, object entity)
        {
            context.Entry(entity).State = EntityState.Deleted;
        }
    }
}
