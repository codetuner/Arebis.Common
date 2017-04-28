using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity
{
    [Serializable]
    public abstract class Entity<TContext>
        where TContext : BaseDbContext<TContext>
    {
        [NonSerialized]
        private TContext context;

        /// <summary>
        /// The context of this entity.
        /// </summary>
        [NotMapped]
        public TContext Context
        {
            get { return this.context; }
            set { this.context = value; }
        }

        /// <summary>
        /// Marks this entity as modified.
        /// </summary>
        public void MarkModified()
        {
            if (this.Context == null)
                throw new InvalidOperationException("No context attached to entity to mark modified.");
            if (this.Context.Entry(this).State == EntityState.Deleted)
                throw new InvalidOperationException("Entity to mark modified is already marked deleted.");

            if (this.Context.Entry(this).State != EntityState.Added)
            {
                this.Context.Entry(this).State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Returns an array with the primary key values of this entity.
        /// The entity must be attached to a context.
        /// </summary>
        protected object[] GetPrimaryKeyValues()
        {
            return this.Context.GetPrimaryKeyValue(this.Context.Entry(this));
        }
    }
}
