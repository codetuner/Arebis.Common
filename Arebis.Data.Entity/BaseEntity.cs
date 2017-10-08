using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Arebis.Data.Entity
{
    [Serializable]
    public abstract class BaseEntity<TContext> : IContextualEntity<TContext>
        where TContext : DbContext
    {
        [NonSerialized]
        private TContext context;

        /// <summary>
        /// The context this entity lives in.
        /// </summary>
        [NotMapped]
        public TContext Context
        {
            get { return this.context; }
            set { this.context = value; }
        }

        /// <summary>
        /// Returns an array with the primary key values of this entity.
        /// The entity must be attached to a context.
        /// </summary>
        protected object[] GetPrimaryKeyValues()
        {
            return this.Context.GetPrimaryKeyValue(this.Context.Entry(this));
        }

        /// <summary>
        /// Override this to perform custom actions before saving.
        /// </summary>
        public virtual void OnSaving(TContext context, DbEntityEntry entry)
        { }

        /// <summary>
        /// Mark this entity as to 'hard' delete on the next SaveChanges.
        /// </summary>
        public virtual void Delete()
        {
            this.Context.Entry(this).State = EntityState.Deleted;
        }
    }
}
