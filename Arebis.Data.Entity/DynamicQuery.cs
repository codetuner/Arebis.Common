using Arebis.Linq;
using Arebis.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Arebis.Data.Entity
{
    public class DynamicQuery<T> where T : class
    {
        const string entityPrefix = "it";

        private DbContext context;
        private string entitySqlBase;
        private List<String> whereStatements = new List<String>();
        private List<String> orderByStatements = new List<string>();
        private List<ObjectParameter> filterParameters = new List<ObjectParameter>();

        public DynamicQuery(DbContext context)
            : this(context, String.Format("OfType({0},{1})", GetEntitySetName<T>(context), typeof(T).FullName))
        { }

        private static string GetEntitySetName<T1>(DbContext context)
        {
            return (context as IObjectContextAdapter).ObjectContext.CreateObjectSet<T>().EntitySet.Name;
        }

        public DynamicQuery(DbContext context, string source)
        {
            this.context = context;
            this.entitySqlBase = "SELECT VALUE it FROM " + source + " AS " + entityPrefix + " ";
        }

        /// <summary>
        /// Add a filter based on a property or property path on the dynamic object query.
        /// </summary>
        public virtual DynamicQuery<T> AddPropertyFilter<TValue>(Expression<Func<T,TValue>> property, object filterValue, string filterValueFormat = "{0}%")
        {
            return this.AddPropertyFilter(property.GetPropertyPath(), filterValue, filterValueFormat);
        }

        /// <summary>
        /// Add a filter based on a property or property path on the dynamic object query.
        /// </summary>
        public virtual DynamicQuery<T> AddPropertyFilter(string filterPropertyPath, object filterValue, string filterValueFormat = "{0}%")
        {
            if (filterValue != null
                && !string.IsNullOrWhiteSpace(filterValue.ToString()))
            {
                string filterParameterName = GetNewParameterName();

                string whereClause = string.Empty;
                if (filterValue is String)
                {
                    whereClause = string.Format("{0}.{1} LIKE @{2}", entityPrefix, filterPropertyPath, filterParameterName);
                }
                else
                {
                    whereClause = string.Format("{0}.{1} = @{2}", entityPrefix, filterPropertyPath, filterParameterName);
                }
                AddFilter(whereClause, filterValue, filterParameterName, filterValueFormat);
            }

            // Fluent support:
            return this;
        }

        /// <summary>
        /// Add a filter based on a property or property path on the dynamic object query.
        /// </summary>
        public virtual DynamicQuery<T> AddPropertyIsNullFilter<TValue>(Expression<Func<T, TValue>> property)
        {
            return this.AddPropertyIsNullFilter(property.GetPropertyPath());
        }

        /// <summary>
        /// Add a filter based on a property or property path on the dynamic object query.
        /// </summary>
        public virtual DynamicQuery<T> AddPropertyIsNullFilter(string filterPropertyPath)
        {
            var whereClause = String.Format("{0}.{1} IS NULL", entityPrefix, filterPropertyPath);
            AddFilter(whereClause);

            // Fluent support:
            return this;
        }

        /// <summary>
        /// Add a filter based on a property or property path on the dynamic object query.
        /// </summary>
        public virtual DynamicQuery<T> AddPropertyIsNotNullFilter<TValue>(Expression<Func<T, TValue>> property)
        {
            return this.AddPropertyIsNotNullFilter(property.GetPropertyPath());
        }

        /// <summary>
        /// Add a filter based on a property or property path on the dynamic object query.
        /// </summary>
        public virtual DynamicQuery<T> AddPropertyIsNotNullFilter(string filterPropertyPath)
        {
            var whereClause = String.Format("{0}.{1} IS NOT NULL", entityPrefix, filterPropertyPath);
            AddFilter(whereClause);

            // Fluent support:
            return this;
        }

        /// <summary>
        /// Add a filter based on the entity type.
        /// </summary>
        public virtual DynamicQuery<T> AddIsOfTypeFilter<TType>()
        {
            var whereClause = String.Format("{0} IS OF ({1}.{2})", entityPrefix, GetEntitySetName<TType>(context), typeof(TType).FullName);
            AddFilter(whereClause);

            // Fluent support:
            return this;
        }

        /// <summary>
        /// Add a filter based on the entity type.
        /// </summary>
        public virtual DynamicQuery<T> AddIsNotOfTypeFilter<TType>()
        {
            var whereClause = String.Format("{0} IS NOT OF ({1}.{2})", entityPrefix, GetEntitySetName<TType>(context), typeof(TType).FullName);
            AddFilter(whereClause);

            // Fluent support:
            return this;
        }

        /// <summary>
        /// Adds an advanced multi-field text filter that can search for multiple values accross multiple fields.
        /// </summary>
        /// <param name="propertyPathsToSearch">Comma-separated list of property paths to be searched. I.e: "FirstName,LastName,Email,Address.Town".</param>
        /// <param name="filterValue">The value(s) to search for. The filterValueSeparator is used to split the value in multiple values.</param>
        /// <param name="supportExplicitPaths">Whether explicit path conditions can be given in the filterValue by passing a value like "PropertyPath=Value". I.e: "Address.Zipcode=1020".</param>
        /// <param name="supportNegation">Whether negative values are supported. I.e: "-ghent" or "Address.Town:-ghent".</param>
        /// <param name="filterValueFormat">Comparison format of the value.</param>
        /// <param name="filterValueSeparator">Separator character for values. By default a space. After splitting, values are always trimmed.</param>
        /// <returns>The query object, for further manipulation.</returns>
        public virtual DynamicQuery<T> AddAdvancedMultiFieldTextFilter(string propertyPathsToSearch, string filterValue, bool supportExplicitPaths = true, bool supportNegation = true, string filterValueFormat = "{0}%", char filterValueSeparator = ' ')
        {
            if (String.IsNullOrEmpty(filterValue)) return this;
            if (String.IsNullOrWhiteSpace(propertyPathsToSearch)) throw new ArgumentException("propertyPathsToSearch parameter cannot be null or empty.", "propertyPathsToSearch");

            var explicitPathValueSeparator = new char[] { '=' };
            
            var properties = propertyPathsToSearch.Split(',');
            var values = filterValue.Split(filterValueSeparator);

            var posWhere = String.Join(" LIKE @@ OR ", properties.Select(p => entityPrefix + "." + p.Trim())) + " LIKE @@";
            var negWhere = String.Join(" NOT LIKE @@ AND ", properties.Select(p => entityPrefix + "." + p.Trim())) + " NOT LIKE @@";

            foreach(var value in values)
            {
                if (String.IsNullOrWhiteSpace(value)) continue;

                string filterParameterName = String.Format("qparam{0}", filterParameters.Count);

                if (supportExplicitPaths && value.Contains(explicitPathValueSeparator[0]))
                {
                    var valueParts = value.Split(explicitPathValueSeparator, 2);
                    var p = valueParts[0].Trim();
                    var v = valueParts[1].Trim();
                    if (v.Length == 0) continue;
                    var pp = typeof(T).GetPropertyPath(p).Last();

                    string whereClause;
                    if (supportNegation && v.StartsWith("-"))
                    {
                        if (pp.PropertyType == typeof(String))
                        {
                            whereClause = String.Format("{0}.{1} NOT LIKE @{2}", entityPrefix, p, filterParameterName);
                        }
                        else
                        {
                            whereClause = String.Format("{0}.{1} <> @{2}", entityPrefix, p, filterParameterName);
                        }
                        AddFilter(whereClause, Convert.ChangeType(v.Substring(1), pp.PropertyType), filterParameterName, filterValueFormat);
                    }
                    else
                    {
                        if (pp.PropertyType == typeof(String))
                        {
                            whereClause = String.Format("{0}.{1} LIKE @{2}", entityPrefix, p, filterParameterName);
                        }
                        else
                        {
                            whereClause = String.Format("{0}.{1} = @{2}", entityPrefix, p, filterParameterName);
                        }
                        AddFilter(whereClause, Convert.ChangeType(v, pp.PropertyType), filterParameterName, filterValueFormat);
                    }
                }
                else
                {
                    var v = value.Trim();
                    if (supportNegation && v.StartsWith("-"))
                    {
                        var whereClause = negWhere.Replace("@@", "@" + filterParameterName);
                        AddFilter(whereClause, v.Substring(1), filterParameterName, filterValueFormat);
                    }
                    else
                    {
                        var whereClause = posWhere.Replace("@@", "@" + filterParameterName);
                        AddFilter(whereClause, v, filterParameterName, filterValueFormat);
                    }
                }
            }

            // Fluent support:
            return this;
        }

        /// <summary>
        /// Add a filter on the dynamic object query.
        /// Parameters can be added and are represented in the whereClause using numbered curly braces "{0}, {1},...".
        /// String parameters are not to be placed between quotes.
        /// Example: AddFilter("Name LIKE {0} And TimeCreated > {1}", "%John%", DateTime.Now);
        /// </summary>
        public virtual DynamicQuery<T> AddFilter(string filterWhereClause, params object[] filterParameterValues)
        {
            var filterParameterNames = new Object[filterParameterValues.Length];
            for(int i=0; i<filterParameterValues.Length; i++)
            {
                var parameterValue = filterParameterValues[i];
                var parameterName = GetNewParameterName();
                filterParameterNames[i] = "@" + parameterName;
                filterParameters.Add(new ObjectParameter(parameterName, parameterValue));
            }

            filterWhereClause = String.Format(filterWhereClause, filterParameterNames);
            whereStatements.Add("(" + filterWhereClause + ")");

            return this;
        }

        /// <summary>
        /// Add a filter on the dynamic object query
        /// </summary>
        private DynamicQuery<T> AddFilter(string filterWhereClause, object filterValue, string filterParameterName, string filterValueFormat = "{0}%")
        {
            if (filterValue != null 
                && !string.IsNullOrWhiteSpace(filterValue.ToString()))
            {
                whereStatements.Add("(" + filterWhereClause + ")");
                if (filterValue is String)
                    filterParameters.Add(new ObjectParameter(filterParameterName, string.Format(filterValueFormat, filterValue)));
                else
                    filterParameters.Add(new ObjectParameter(filterParameterName, filterValue));
            }

            // Fluent support:
            return this;
        }

        /// <summary>
        ///Add a filter on the dynamic query
        /// </summary>
        public virtual DynamicQuery<T> AddFilter(string filterWhereClause)
        {
            if (!string.IsNullOrWhiteSpace(filterWhereClause))
            {
                whereStatements.Add("(" + filterWhereClause + ")");
            }

            // Fluent support:
            return this;
        }

        /// <summary>
        /// Add a sorting on the dynamic object query
        /// </summary>
        public virtual DynamicQuery<T> AddSort<TValue>(Expression<Func<T, TValue>> orderBy, bool ascending = true)
        {
            return this.AddSort(orderBy.GetPropertyPath(), ascending);
        }

        /// <summary>
        /// Add a sorting on the dynamic object query
        /// </summary>
        /// <param name="commaSeparatedListOfFields">Comma-separated list of fieldnames to sort on. I.e. "LastName,FirstName".</param>
        /// <param name="ascending">Whether to sort ascending (false = descending).</param>
        public virtual DynamicQuery<T> AddSort(string commaSeparatedListOfFields, bool ascending = true)
        {
            if (!String.IsNullOrWhiteSpace(commaSeparatedListOfFields))
            {
                foreach (var field in commaSeparatedListOfFields.Split(','))
                {
                    if (String.IsNullOrWhiteSpace(field)) continue;
                    string orderByClause = string.Format("{0}.{1}", entityPrefix, field.Trim());
                    orderByStatements.Add(orderByClause + (ascending ? " ASC" : " DESC"));
                }
            }

            // Fluent support:
            return this;
        }

        /// <summary>
        /// Generates the EntitySQL query string.
        /// </summary>
        public virtual string GetQueryString()
        {
            // Start with the entitySQLBase:
            var queryString = entitySqlBase;

            // Add the where-statements:
            for (int i = 0; i < whereStatements.Count; i++)
            {
                queryString += (i == 0 ? " WHERE " : " AND ") + whereStatements[i];
            }

            // Add the orderby-statements:
            for (int i = 0; i < orderByStatements.Count; i++)
            {
                queryString += (i == 0 ? " ORDER BY " : " , ") + orderByStatements[i];
            }

            return queryString;
        }

        /// <summary>
        /// Generates the ObjectQuery.
        /// </summary>
        public virtual ObjectQuery<T> GetQuery()
        {
            // Create the ObjectQuery:
            var returnValue = ((IObjectContextAdapter)this.context).ObjectContext.CreateQuery<T>(GetQueryString(), filterParameters.ToArray());
            
            // Return the query:
            return returnValue;
        }

        /// <summary>
        /// Returns the number of objects matching the current query.
        /// </summary>
        public virtual int GetCount()
        {
            return this.GetQuery().Count();
        }

        /// <summary>
        /// Returns an unused parameter name.
        /// </summary>
        private string GetNewParameterName()
        {
            return String.Format("qparam{0}", filterParameters.Count);
        }
    }
}
