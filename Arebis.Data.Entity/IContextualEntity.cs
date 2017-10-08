using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Arebis.Data.Entity
{
    /// <summary>
    /// An interface that represents the awareness of an entity to its context.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IContextualEntity<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// The context owning this entity instance.
        /// </summary>
        TContext Context { get; set; }

        /// <summary>
        /// Called before saving this entity.
        /// </summary>
        void OnSaving(TContext context, DbEntityEntry entry);
    }
}
