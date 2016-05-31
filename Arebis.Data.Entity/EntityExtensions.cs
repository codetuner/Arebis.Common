using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Arebis.Data.Entity
{
    /// <summary>
    /// ADO.NET Entity Framework cross cutting extension helper methods
    /// </summary>
    public static class EntityExtensions
    {
        public static long? GetLastGeneratedIdentity(this System.Data.Entity.Core.EntityClient.EntityConnection connection)
        {
            var storeConnection = connection.StoreConnection;
            if (storeConnection is System.Data.SqlClient.SqlConnection) return ((System.Data.SqlClient.SqlConnection)storeConnection).GetLastGeneratedIdentity();
            else if (storeConnection is System.Data.OleDb.OleDbConnection) return ((System.Data.OleDb.OleDbConnection)storeConnection).GetLastGeneratedIdentity();
            else throw new NotSupportedException("Only EntityConnections to SqlConnections and OleDbConnections are supported so far.");
        }

        [Arebis.Source.CodeSource("http://blogs.u2u.be/diederik/post/2010/08/30/Getting-and-setting-the-Transaction-Isolation-Level-on-a-SQL-Entity-Connection.aspx")]
        public static IsolationLevel GetIsolationLevel(this System.Data.Entity.Core.EntityClient.EntityConnection connection)
        {
            var sqlConnection = connection.StoreConnection as System.Data.SqlClient.SqlConnection;
            if (sqlConnection != null) return sqlConnection.GetIsolationLevel();
            else throw new NotSupportedException("Only EntityConnections to SqlConnections are supported so far.");
        }

        [Arebis.Source.CodeSource("http://blogs.u2u.be/diederik/post/2010/08/30/Getting-and-setting-the-Transaction-Isolation-Level-on-a-SQL-Entity-Connection.aspx")]
        public static void SetIsolationLevel(this System.Data.Entity.Core.EntityClient.EntityConnection connection, IsolationLevel isolationLevel)
        {
            var sqlConnection = connection.StoreConnection as System.Data.SqlClient.SqlConnection;
            if (sqlConnection != null) sqlConnection.SetIsolationLevel(isolationLevel);
            else throw new NotSupportedException("Only EntityConnections to SqlConnections are supported so far.");
        }

        /// <summary> 
        /// Get referenced entity key values
        /// </summary> 
        /// <param name="entityReference">entity reference property</param> 
        /// <remarks> 
        /// E.g. int id = (int)customer.CountryReference.GetEntityKey()[0];
        /// </remarks> 
        public static object[] GetEntityKey(this EntityReference entityReference)
        {

            object[] returnValue = null;
            if (entityReference != null && entityReference.EntityKey != null)
            {
                returnValue = new object[entityReference.EntityKey.EntityKeyValues.Length];
                for(int i=0; i<returnValue.Length; i++)
                    returnValue[i] = entityReference.EntityKey.EntityKeyValues[i].Value;
            }
            return returnValue;
        }

        /// <summary> 
        /// Sets the EntityKey on an entity 
        /// </summary> 
        /// <typeparam name="TEntity">type of the entity</typeparam> 
        /// <param name="entityReference">entity reference property</param> 
        /// <param name="keyValues">Key value(s) to set. 
        /// Multiple keys might be provided for entities with multiple key columns.</param> 
        /// <remarks> 
        /// E.g. customer.CountryReference.SetEntityKey( "B" [, "OtherKeyColumnValue"] ) 
        /// </remarks> 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetEntityKey<TEntity>(this EntityReference<TEntity> entityReference, params object[] keyValues) where TEntity : class, IEntityWithRelationships
        {

            var qualifiedName = string.Empty;
            var entitySet = GetEntitySet(typeof(TEntity));
            if (entitySet != null)
            {
                if (keyValues.Length == entitySet.ElementType.KeyMembers.Count)
                {
                    qualifiedName = entitySet.EntityContainer.Name + "." + entitySet.Name;
                    var keys = new List<EntityKeyMember>();
                    int argnum = 0;
                    for (argnum = 0; argnum <= keyValues.Length - 1; argnum++)
                    {
                        keys.Add(new EntityKeyMember(entitySet.ElementType.KeyMembers[argnum].Name, keyValues[argnum]));
                    }
                    // Finally set the entity key 
                    entityReference.EntityKey = new EntityKey(qualifiedName, keys);
                }
                else
                {
                    // This exception will typically be thrown in a design time 
                    // environment after integration testing and model change 
                    throw new ArgumentException("Invalid Number Of KeyValues", "keyValues");
                }
            }
        }

        /// <summary> 
        /// Gets the name of the entity container 
        /// </summary> 
        /// <param name="entity">entity</param> 
        /// <returns>name of the entity container</returns> 
        public static string GetEntityContainerName(this IEntityWithRelationships entity)
        {
            var result = string.Empty;
            if (entity != null)
            {
                var entitySet = GetEntitySet(entity.GetType());
                if (entitySet != null)
                {
                    result = entitySet.EntityContainer.Name;
                }
            }
            return result;
        }

        /// <summary>
        /// Adds the given entity to the context.
        /// </summary>
        public static void AddObject(this ObjectContext context, object entity)
        {
            context.AddObject(context.GetEntitySetName(entity.GetType()), entity);
        }

        /// <summary>
        /// Returns the EntitySetName for the given entity type.
        /// </summary>
        public static string GetEntitySetName(this ObjectContext context, Type entityType)
        {
            Type type = entityType;

            while (type != null)
            {
                // Use first EdmEntityTypeAttribute found:
                foreach (EdmEntityTypeAttribute typeattr in type.GetCustomAttributes(typeof(EdmEntityTypeAttribute), false))
                {
                    // Retrieve the entity container for the conceptual model:
                    var container = context.MetadataWorkspace.GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace);

                    // Retrieve the name of the entityset matching the given types EdmEntityTypeAttribute:
                    string entitySetName = (from meta in container.BaseEntitySets
                                            where meta.ElementType.FullName == typeattr.NamespaceName + "." + typeattr.Name
                                            select meta.Name)
                                            .FirstOrDefault();

                    // If match, return the entitySetName:
                    if (entitySetName != null) return entitySetName;
                }

                // If no matching attribute or entitySetName found, try basetype:
                type = type.BaseType;
            }

            // Fail if no valid entitySetName was found:
            throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Unable to determine EntitySetName of type '{0}'.", entityType));
        }

        /// <summary> 
        /// Keeps track of the EDM metadata 
        /// </summary> 
        /// <remarks></remarks> 
        private static MetadataWorkspace metaData = LoadMetaDataFromResource();

        /// <summary> 
        /// Loads the EDM metadata from embedded resources in assemlies used 
        /// in the project 
        /// </summary> 
        [Arebis.Source.CodeToDo("Remove obsolete call to RegisterItemCollection.")]
        private static MetadataWorkspace LoadMetaDataFromResource()
        {
            // Load EDM meta data from resources in all used assemblies 
            var workspace = new MetadataWorkspace();
            try
            {
                var metaDataColl = new EdmItemCollection("res://*/");
                workspace.RegisterItemCollection(metaDataColl); // TODO: Replace by non-obsolete variant.
            }
            catch (Exception)
            {
                // Log a critical message - in case of inproper 
                // exception handling in the application 
                //TODO Log a critical message - in case of inproper exception handling in the application 
                ////var message = string.Format(CultureInfo.InvariantCulture, FrameworkMessage.ErrorLoadingEdmMetaData, ex.ToString());
                ////Logger.Write(message, TraceEventType.Critical);
                // Rethrow 
                throw;
            }
            return workspace;
        }

        /// <summary> 
        /// Gets the metadata for an entityset using the .NET type information of an entity 
        /// </summary> 
        /// <param name="entityType">.NET CLR type information of an entity</param> 
        /// <returns>metatdata for the passed entity or nothing if not found</returns> 
        /// <remarks> 
        /// </remarks> 
        private static EntitySetBase GetEntitySet(Type entityType)
        {
            EntitySetBase result = null;
            if (entityType != null && metaData != null)
            {
                // Get attribute of entity type using reflection and LINQ on attributes 
                EdmEntityTypeAttribute edmAttrib = default(EdmEntityTypeAttribute);
                edmAttrib = entityType.GetCustomAttributes(typeof(EdmEntityTypeAttribute), true).FirstOrDefault() as EdmEntityTypeAttribute;
                ////Debug.Assert(edmAttrib != null); 

                // Get entity containters for metadata 
                var globalItems = metaData.GetItems(DataSpace.CSpace);

                // LINQ query on metadata conceptual space 
                var query2 = from c in globalItems
                             where c.BuiltInTypeKind == BuiltInTypeKind.EntityContainer
                             from s in ((EntityContainer)c).BaseEntitySets
                             where s.ElementType.Name == edmAttrib.Name
                                   && s.ElementType.NamespaceName == edmAttrib.NamespaceName
                             select (EntitySetBase)s;
                result = query2.FirstOrDefault();
            }
            return result;
        }

        /// <summary>
        /// Add an order by based on a string
        /// </summary>
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string propertyName, bool asc) where TEntity : class
        {
            var type = typeof(TEntity);
            string methodName = asc ? "OrderBy" : "OrderByDescending";
            MethodCallExpression resultExp = null;
            if (!propertyName.Contains("."))
            {
                var property = type.GetProperty(propertyName);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), methodName,
                new Type[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExp));
            }
            else
            {
                Type relationType = type.GetProperty(propertyName.Split('.')[0]).PropertyType;
                PropertyInfo relationProperty = type.GetProperty(propertyName.Split('.')[0]);
                PropertyInfo relationProperty2 = relationType.GetProperty(propertyName.Split('.')[1]);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, relationProperty);
                var propertyAccess2 = Expression.MakeMemberAccess(propertyAccess, relationProperty2);
                var orderByExp = Expression.Lambda(propertyAccess2, parameter);
                resultExp = Expression.Call(typeof(Queryable), methodName,
                new Type[] { type, relationProperty2.PropertyType },
                source.Expression, Expression.Quote(orderByExp));

            }
            return source.Provider.CreateQuery<TEntity>(resultExp);
        }

        /// <summary>
        /// Add a ThenBy based on a string
        /// </summary>
        public static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IOrderedQueryable<TEntity> source, string propertyName, bool asc) where TEntity : class
        {
            var type = typeof(TEntity);
            string methodName = asc ? "ThenBy" : "ThenByDescending";
            MethodCallExpression resultExp = null;
            if (!propertyName.Contains("."))
            {
                var property = type.GetProperty(propertyName);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), methodName,
                new Type[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExp));
            }
            else
            {
                Type relationType = type.GetProperty(propertyName.Split('.')[0]).PropertyType;
                PropertyInfo relationProperty = type.GetProperty(propertyName.Split('.')[0]);
                PropertyInfo relationProperty2 = relationType.GetProperty(propertyName.Split('.')[1]);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, relationProperty);
                var propertyAccess2 = Expression.MakeMemberAccess(propertyAccess, relationProperty2);
                var orderByExp = Expression.Lambda(propertyAccess2, parameter);
                resultExp = Expression.Call(typeof(Queryable), methodName,
                new Type[] { type, relationProperty2.PropertyType },
                source.Expression, Expression.Quote(orderByExp));
            }
            return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(resultExp);
        }

        /// <summary>
        /// Filter a queryable object
        /// </summary>
        /// <typeparam name="T">type of the entity</typeparam>
        /// <param name="source">Queryable source</param>
        /// <param name="fieldName">Field to filter on</param>
        /// <param name="value">Value to filter on</param>
        /// <returns>Filtered queryable</returns>
        public static IQueryable<T> Search<T>(this IQueryable<T> source, string fieldName, string value)
        {
            MethodInfo mi = typeof(String).GetMethod("StartsWith", new Type[] { typeof(String) });
            ParameterExpression X = Expression.Parameter(typeof(T), "x");
            MemberExpression field = Expression.PropertyOrField(X, fieldName);
            MethodCallExpression startsWith = Expression.Call(field, mi, Expression.Constant(value));
            Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(startsWith, X);
            return source.Where(expression);
        }

        /// <summary>
        /// Simulate an IN
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <typeparam name="TValue">Type of the value to search for</typeparam>
        /// <param name="source">Queryable source</param>
        /// <param name="valueSelector">Value selector lambda expression</param>
        /// <param name="values">Values to select</param>
        /// <returns>Filtered queryable</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IQueryable<T> ValueWithinRange<T, TValue>(this IQueryable<T> source, Expression<Func<T, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == valueSelector) { throw new ArgumentNullException("valueSelector"); }
            if (null == values) { throw new ArgumentNullException("values"); }
            ParameterExpression p = valueSelector.Parameters.Single();
            if (!values.Any())
            {
                return source.Where(x => false);
            }
            var equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
            Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(body, p);
            return source.Where(expression);
        }

        /// <summary>
        /// Specifies the related objects to include in the query results using
        /// a lambda expression listing the path members.
        /// </summary>
        /// <returns>A new System.Data.Objects.ObjectQuery&lt;T&gt; with the defined query path.</returns>
        public static IQueryable<T> Include<T>(this IQueryable<T> query, Expression<Func<T, object>> path)
        {
            // If it's a System.Data.Objects.ObjectQuery, apply the include, otherwise ignore:
            if (query is ObjectQuery<T>)
            {
                // Retrieve member path:
                List<ExtendedPropertyInfo> members = new List<ExtendedPropertyInfo>();
                EntityFrameworkHelper.CollectRelationalMembers(path, members);

                // Build string path:
                StringBuilder sb = new StringBuilder();
                string separator = "";
                foreach (ExtendedPropertyInfo member in members)
                {
                    if (member.ReferenceOnly) break;
                    sb.Append(separator);
                    sb.Append(member.PropertyInfo.Name);
                    separator = ".";
                }

                // Apply Include:
                if (sb.Length > 0)
                    return ((ObjectQuery<T>)query).Include(sb.ToString());
                else
                    return query;
            }
            else
            {
                // Object is not an EF ObjectQuery:
                return query;
            }
        }

        /// <summary>
        /// Get the sql from a linq to entities query
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>String with the query</returns>
        public static string ToTraceString(this IQueryable query)
        {
            System.Reflection.MethodInfo toTraceStringMethod = query.GetType().GetMethod("ToTraceString");
            if (toTraceStringMethod != null)
                return toTraceStringMethod.Invoke(query, null).ToString();
            else
                return "";
        }

        /// <summary>
        /// Returns the object graph as detached from its context.
        /// </summary>
        /// <remarks>
        /// This detach method provides a workaround for the Entity Framework 'NoTracking Memory Leak'
        /// described in http://social.msdn.microsoft.com/Forums/en-SG/adodotnetentityframework/thread/906c0cad-840b-4eb8-ba11-5348d407df73
        /// by performing its detaching using serialization/deserialization.
        /// </remarks>
        public static TEntity DetachGraph<TEntity>(this TEntity entity) where TEntity : EntityObject
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, entity);
                stream.Seek(0, SeekOrigin.Begin);
                return (TEntity)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Returns an array with the object graphs as detached from their context.
        /// </summary>
        /// <remarks>
        /// This detach method provides a workaround for the Entity Framework 'NoTracking Memory Leak'
        /// described in http://social.msdn.microsoft.com/Forums/en-SG/adodotnetentityframework/thread/906c0cad-840b-4eb8-ba11-5348d407df73
        /// by performing its detaching using serialization/deserialization.
        /// </remarks>
        public static TEntity[] DetachGraphCollection<TEntity>(this IEnumerable<TEntity> entities) where TEntity : EntityObject
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, entities.ToArray());
                stream.Seek(0, SeekOrigin.Begin);
                return (TEntity[])formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Returns the object context the current entity is attached to.
        /// </summary>
        public static ObjectContext GetObjectContext(this EntityObject entity)
        { 
            // entity._realtionships._context
            if (entity == null)
                throw new ArgumentNullException("entity", "Entity is null.");

            // For detached objects, return null:
            if (entity.EntityState == System.Data.Entity.EntityState.Detached)
                return null;

            // Retrieve RelationshipManager:
            var relationShipManager = ((IEntityWithRelationships)entity).RelationshipManager;
            if (relationShipManager == null) return null;

            // Try retrieving context - EF 4.0 way:
            PropertyInfo wrappedOwnerProp = typeof(RelationshipManager).GetProperty("WrappedOwner", BindingFlags.Instance | BindingFlags.NonPublic);
            if (wrappedOwnerProp != null)
            {
                object wrappedOwner = wrappedOwnerProp.GetValue(relationShipManager, null);
                if (wrappedOwner == null) return null;
                FieldInfo contextField = wrappedOwner.GetType().BaseType.GetField("<Context>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                if (contextField == null) goto failNotSupported;
                object context = contextField.GetValue(wrappedOwner);
                return (ObjectContext)context;
            }

            // Try retrieving context - EF 1.0 way:
            PropertyInfo contextProp = typeof(RelationshipManager).GetProperty("Context", BindingFlags.Instance | BindingFlags.NonPublic);
            if (contextProp != null) {
                object context = contextProp.GetValue(relationShipManager, null);
                return (ObjectContext)context;
            }

            // If none succeeded:
            failNotSupported:
            throw new NotSupportedException("Operation not supported by this version of the CLR.");
        }

        /// <summary>
        /// Skip and take in one move. If take argument is -1, all rows are returned after the skip operation.
        /// </summary>
        public static IQueryable<TEntity> SkipAndTake<TEntity>(this IQueryable<TEntity> query, int skip, int take)
        {
            query = query.Skip(skip);
            if (take > -1)
                query = query.Take(take);
            return query;
        }
    }
}
