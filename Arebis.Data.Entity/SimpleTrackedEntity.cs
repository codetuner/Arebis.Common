using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;

namespace Arebis.Data.Entity
{
    public abstract class SimpleTrackedEntity<TContext> : BaseEntity<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// UTC time this entity was created.
        /// </summary>
        //[ScaffoldColumn(false)]
        public virtual DateTime CreatedTimeUtc { get; set; }

        /// <summary>
        /// UTC time this entity was last updated.
        /// </summary>
        //[ScaffoldColumn(false)]
        public virtual DateTime UpdatedTimeUtc { get; set; }

        /// <summary>
        /// UTC time this entity was deleted.
        /// </summary>
        //[ScaffoldColumn(false)]
        public virtual DateTime? DeletedTimeUtc { get; set; }

        /// <summary>
        /// Name/identifier of the user who created or updated the entity as last.
        /// </summary>
        //[ScaffoldColumn(false)]
        public virtual string UpdatedBy { get; set; }

        /// <summary>
        /// Whether this entity is marked deleted.
        /// </summary>
        [NotMapped]
        public bool IsDeleted
        {
            get
            {
                return (this.DeletedTimeUtc.HasValue);
            }
        }

        /// <summary>
        /// Mark this entity as to 'soft' delete on the next SaveChanges.
        /// </summary>
        public override void Delete()
        {
            this.DeletedTimeUtc = Current.DateTime.UtcNow;
        }

        /// <summary>
        /// Mark then entity as not deleted anymore.
        /// </summary>
        public virtual void Undelete()
        {
            this.DeletedTimeUtc = null;
        }

        /// <summary>
        /// Called before saving the entity.
        /// </summary>
        /// 
        public override void OnSaving(TContext context, DbEntityEntry entry)
        {
            base.OnSaving(context, entry);

            if (entry.State == EntityState.Added)
            {
                this.CreatedTimeUtc = this.UpdatedTimeUtc = Current.DateTime.UtcNow;
                this.UpdatedBy = Thread.CurrentPrincipal.Identity.Name;
            }
            else if (entry.State == EntityState.Modified)
            {
                this.UpdatedTimeUtc = Current.DateTime.UtcNow;
                this.UpdatedBy = Thread.CurrentPrincipal.Identity.Name;
            }
        }
    }
}
