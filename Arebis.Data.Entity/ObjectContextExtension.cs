using Arebis.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Arebis.Data.Entity
{
	/// <summary>
	/// Extension methods on ObjectContext.
	/// </summary>
	public static class ObjectContextExtension
	{
		/// <summary>
		/// Attaches an entire objectgraph to the context.
		/// </summary>
		public static T AttachObjectGraph<T>(this ObjectContext context, T entity, params Expression<Func<T, object>>[] paths)
		{
			return AttachObjectGraphs(context, new T[] { entity }, paths)[0];
		}

		/// <summary>
		/// Attaches multiple entire objectgraphs to the context.
		/// </summary>
		public static T[] AttachObjectGraphs<T>(this ObjectContext context, IEnumerable<T> entities, params Expression<Func<T, object>>[] paths)
		{
			T[] unattachedEntities = entities.ToArray();
			T[] attachedEntities = new T[unattachedEntities.Length];
			Type entityType = typeof(T);

			if (unattachedEntities.Length > 0)
			{
				// Workaround to ensure the assembly containing the entity type is loaded:
				// (see: https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=3405138&SiteID=1)
				try { context.MetadataWorkspace.LoadFromAssembly(entityType.Assembly); }
				catch { }

				#region Automatic preload root entities

				// Create a WHERE clause for preload the root entities:
				StringBuilder where = new StringBuilder("(1=0)");
				List<ObjectParameter> pars = new List<ObjectParameter>();
				int pid = 0;
				foreach (T entity in unattachedEntities)
				{
					// If the entity has an entitykey:
					EntityKey entityKey = ((IEntityWithKey)entity).EntityKey;
					if (entityKey != null)
					{
						where.Append(" OR ((1=1)");
						foreach (EntityKeyMember keymember in entityKey.EntityKeyValues)
						{
							string pname = String.Format("p{0}", pid++);
							where.Append(" AND (it.[");
							where.Append(keymember.Key);
							where.Append("] = @");
							where.Append(pname);
							where.Append(")");
							pars.Add(new ObjectParameter(pname, keymember.Value));
						}
						where.Append(")");
					}
				}

				// If WHERE clause not empty, construct and execute query:
				if (pars.Count > 0)
				{
					// Construct query:
					ObjectQuery<T> query = (ObjectQuery<T>)context.PublicGetProperty(context.GetEntitySetName(typeof(T)));
					foreach (var path in paths)
                        query = (ObjectQuery<T>)query.Include(path);
					query = query.Where(where.ToString(), pars.ToArray());

					// Execute query and load entities:
					//Console.WriteLine(query.ToTraceString());
					query.Execute(MergeOption.AppendOnly).ToArray();
				}

				#endregion Automatic preload root entities

				// Attach the root entities:
				for (int i = 0; i < unattachedEntities.Length; i++)
					attachedEntities[i] = (T)context.AddOrAttachInstance(unattachedEntities[i], true);

				// Collect property paths into a tree:
				TreeNode<ExtendedPropertyInfo> root = new TreeNode<ExtendedPropertyInfo>(null);
				foreach (var path in paths)
				{
					List<ExtendedPropertyInfo> members = new List<ExtendedPropertyInfo>();
					EntityFrameworkHelper.CollectRelationalMembers(path, members);
					root.AddPath(members);
				}

				// Navigate over all properties:
				for (int i = 0; i < unattachedEntities.Length; i++)
					NavigatePropertySet(context, root, unattachedEntities[i], attachedEntities[i]);
			}

			// Return the attached root entities:
			return attachedEntities;
		}

		/// <summary>
		/// Adds or attaches the entity to the context. If the entity has an EntityKey,
		/// the entity is attached, otherwise a clone of it is added.
		/// </summary>
		/// <returns>The attached entity.</returns>
		public static object AddOrAttachInstance(this ObjectContext context, object entity, bool applyPropertyChanges)
		{
			EntityKey entityKey = ((IEntityWithKey)entity).EntityKey;
			if (entityKey == null)
			{
				object attachedEntity = GetShallowEntityClone(entity);
				context.AddObject(context.GetEntitySetName(entity.GetType()), attachedEntity);
				((IEntityWithKey)entity).EntityKey = ((IEntityWithKey)attachedEntity).EntityKey;
				return attachedEntity;
			}
			else
			{
				object attachedEntity = context.GetObjectByKey(entityKey);
				if (applyPropertyChanges)
					context.ApplyCurrentValues(entityKey.EntitySetName, entity);
				return attachedEntity;
			}
		}

		/// <summary>
		/// Detaches an objectgraph given it's root object.
		/// </summary>
		/// <returns>The detached root object.</returns>
		public static T DetachObjectGraph<T>(this ObjectContext context, T entity)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				//NetDataContractSerializer serializer = new NetDataContractSerializer();
				//serializer.Serialize(stream, entity);
				//stream.Position = 0;
				//return (T)serializer.Deserialize(stream);
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, entity);
				stream.Position = 0;
				return (T)formatter.Deserialize(stream);
			}
		}

		#region Entity path marker methods

		/// <summary>
		/// Marker method to indicate this section of the path expression
		/// should not be loaded but only referenced.
		/// </summary>
		public static object ReferenceOnly(this IEntityWithKey entity)
		{
			throw new InvalidOperationException("The ReferenceOnly() method is a marker method in entity property paths and should not be effectively invoked.");
		}

		/// <summary>
		/// Marker method to indicate the instances the method is called on
		/// within path expressions should not be updated.
		/// </summary>
		public static object WithoutUpdate(this IEntityWithKey entity)
		{
			throw new InvalidOperationException("The WithoutUpdate() method is a marker method in entity property paths and should not be effectively invoked.");
		}

		#endregion

		#region Private implementation

		/// <summary>
		/// Navigates a property path on detached instance to translate into attached instance.
		/// </summary>
		private static void NavigatePropertySet(ObjectContext context, TreeNode<ExtendedPropertyInfo> propertynode, object owner, object attachedowner)
		{
			// Try to navigate each of the properties:
			foreach (TreeNode<ExtendedPropertyInfo> childnode in propertynode.Children)
			{
				ExtendedPropertyInfo property = childnode.Item;

				// Retrieve property value:
				object related = property.PropertyInfo.GetValue(owner, null);

				if ((property.ReferenceOnly) && (typeof(IEnumerable).IsAssignableFrom(property.PropertyInfo.PropertyType)))
				{
					// ReferenceOnly marker not valid on collections:
					throw new InvalidOperationException("The ReferenceOnly marker method is not supported on the many side of relations.");
				}
				else if (property.ReferenceOnly)
				{
					// Apply reference update on ReferenceOnly:
					EntityReference reference = (EntityReference)attachedowner.PublicGetProperty(property.PropertyInfo.Name + "Reference");
					reference.EntityKey = ((EntityReference)owner.PublicGetProperty(property.PropertyInfo.Name + "Reference")).EntityKey;
				}
				else if (related is IEnumerable)
				{
					// Load current list in context:
					object attachedlist = property.PropertyInfo.GetValue(attachedowner, null);
					RelatedEnd relatedEnd = (RelatedEnd)attachedlist;
					if (((EntityObject)attachedowner).EntityState != EntityState.Added && !relatedEnd.IsLoaded)
						relatedEnd.Load();

					// Recursively navigate through new members:
					List<object> newlist = new List<object>();
					foreach (var relatedinstance in (IEnumerable)related)
					{
						object attachedinstance = context.AddOrAttachInstance(relatedinstance, !property.NoUpdate);
						newlist.Add(attachedinstance);
						NavigatePropertySet(context, childnode, relatedinstance, attachedinstance);
					}

					// Synchronise lists:
					List<object> removedItems;
					SyncList(attachedlist, newlist, out removedItems);

					// Delete removed items if association is owned:
					if (AssociationEndBehaviorAttribute.GetAttribute(property.PropertyInfo).Owned)
					{
						foreach (var removedItem in removedItems)
							context.DeleteObject(removedItem);
					}

				}
				else if (!typeof(IEnumerable).IsAssignableFrom(property.PropertyInfo.PropertyType))
				{
					// Load reference of currently attached in context:
					RelatedEnd relatedEnd = (RelatedEnd)attachedowner.PublicGetProperty(property.PropertyInfo.Name + "Reference");
					if (((EntityObject)attachedowner).EntityState != EntityState.Added && !relatedEnd.IsLoaded)
						relatedEnd.Load();

					// Recursively navigate through new value (unless it's null):
					object attachedinstance;
					if (related == null)
						attachedinstance = null;
					else
					{
						attachedinstance = context.AddOrAttachInstance(related, !property.NoUpdate);
						NavigatePropertySet(context, childnode, related, attachedinstance);
					}

					// Synchronise value:
					property.PropertyInfo.SetValue(attachedowner, attachedinstance, null);
				}
			}
		}

		/// <summary>
		/// Returns a shallow clone of only the scalar properties.
		/// </summary>
		private static object GetShallowEntityClone(object entity)
		{
			object clone = Activator.CreateInstance(entity.GetType());
			foreach (PropertyInfo prop in entity.GetType().GetProperties())
			{
				if (typeof(RelatedEnd).IsAssignableFrom(prop.PropertyType)) continue;
				//if (typeof(EntityReference).IsAssignableFrom(prop.PropertyType)) continue;
				//if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && (!typeof(String).IsAssignableFrom(prop.PropertyType))) continue;
				if (typeof(IEntityWithKey).IsAssignableFrom(prop.PropertyType)) continue;
				try
				{
					prop.SetValue(clone, prop.GetValue(entity, null), null);
				}
				catch
				{
				}
			}
			return clone;
		}

		/// <summary>
		/// Synchronises a targetlist with a sourcelist by adding or removing items from the targetlist.
		/// The targetlist is untyped and controlled through reflection.
		/// </summary>
		private static void SyncList(object targetlist, List<object> sourcelist, out List<object> removedItems)
		{
			List<object> localsourcelist = new List<object>(sourcelist);
			List<object> toremove = new List<object>();

			// Compare both lists:
			foreach (object item in (IEnumerable)targetlist)
			{
				bool found = false;
				for (int i = 0; i < localsourcelist.Count; i++)
				{
					if (Object.ReferenceEquals(localsourcelist[i], item))
					{
						localsourcelist[i] = null;
						found = true;
					}
				}
				if (!found)
					toremove.Add(item);
			}

			// Add members not in targetlist:
			foreach (object item in localsourcelist)
			{
				if (Object.ReferenceEquals(item, null) == false)
					targetlist.PublicInvokeMethod("Add", item);
			}

			// Remove members not in sourcelist:
			foreach (object item in toremove)
				targetlist.PublicInvokeMethod("Remove", item);

			// Expose removed items:
			removedItems = toremove;
		}

		#endregion
	}
}
