using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Arebis.Extensions;
using System.Text.RegularExpressions;

namespace Arebis.Data
{
    /// <summary>
    /// Mapper to perform SQL queries and return results as mapped to typed objects or dynamic ExpandoObjects.
    /// </summary>
    public class QueryMapper : IDisposable
    {
        private static Regex numerical = new Regex("^[0-9]+$", RegexOptions.Compiled);

        public QueryMapper(DbConnection connection, string sql, CommandType commandType = CommandType.Text)
        {
            if (connection.State != ConnectionState.Open) connection.Open();
            this.Command = connection.CreateCommand();
            this.Command.CommandText = sql;
            this.Command.CommandType = commandType;
        }

        public DbConnection Connection { get; private set; }

        public DbCommand Command { get; private set; }

        private DbDataReader reader;

        public DbDataReader Reader
        {
            get
            {
                if (this.reader == null)
                {
                    this.reader = this.Command.ExecuteReader();
                }
                return this.reader;
            }
        }

        protected void AssertNoReader()
        {
            if (this.reader != null) throw new Exception("Cannot set parameters on QueryMapper once reader is created.");
        }

        public QueryMapper WithParameters(object parameterObject, string parameterNamePrefix = "@", string parameterNameSuffix = "")
        {
            return this.WithParameters(parameterObject.ToDictionary(), parameterNamePrefix, parameterNameSuffix);
        }

        public QueryMapper WithParameters(IDictionary<string, object> parameters, string parameterNamePrefix = "@", string parameterNameSuffix = "")
        {
            AssertNoReader();

            if (parameters != null)
            {
                foreach (var pair in parameters)
                {
                    this.Command.AddParameter(parameterNamePrefix + pair.Key + parameterNameSuffix, pair.Value);
                }
            }
            return this; // to support fluent notation.
        }

        public QueryMapper WithParameter(string name, object value, ParameterDirection direction = ParameterDirection.Input, DbType dbType = DbType.String, int? size = null)
        {
            AssertNoReader();

            this.Command.AddParameter(name, value, direction, dbType, size);

            return this; // to support fluent notation.
        }

        public QueryMapper WithParameter(DbParameter parameter)
        {
            AssertNoReader();

            this.Command.Parameters.Add(parameter);

            return this; // to support fluent notation.
        }

        public QueryMapper Skip(int rowcount)
        {
            for (int r = 0; r < rowcount; r++)
            {
                if (!Reader.Read()) break;
            }

            return this; // to support fluent notation.
        }

        /// <summary>
        /// Maps all rows to ExpandoObjects.
        /// </summary>
        /// <returns>An enumeration of ExpandoOjects.</returns>
        public IEnumerable<ExpandoObject> TakeAll()
        {
            return this.Take(Int32.MaxValue);
        }

        /// <summary>
        /// Take rowcount number of rows and maps them to ExpandoObjects.
        /// </summary>
        /// <param name="rowcount">Up to number of rows to return.</param>
        /// <returns>An enumeration of ExpandoOjects.</returns>
        public IEnumerable<ExpandoObject> Take(int rowcount)
        {
            // Retrieve fields names:
            var fieldcount = Reader.FieldCount;
            var fields = new string[fieldcount];
            for (int f = 0; f < fields.Length; f++)
            {
                fields[f] = Reader.GetName(f);
            }

            // Map rows to ExpandoObjects:
            for (int r = 0; r < rowcount; r++)
            {
                if (!Reader.Read()) break;

                var obj = (IDictionary<string, object>)new ExpandoObject();
                for (int c = 0; c < fieldcount; c++)
                {
                    obj[fields[c]] = (Reader.IsDBNull(c) ? null : Reader.GetValue(c));
                }
                yield return (ExpandoObject)obj;
            }
        }

        /// <summary>
        /// Maps all rows to objects of type T.
        /// Numerical column names map to default indexer properties.
        /// </summary>
        /// <typeparam name="T">Type of objects to return.</typeparam>
        /// <returns>An enumeration of T objects.</returns>
        public IEnumerable<T> TakeAll<T>()
            where T : new()
        {
            return this.Take<T>(Int32.MaxValue);
        }

        /// <summary>
        /// Take rowcount number of rows and maps them to objects of type T.
        /// Numerical column names map to default indexer properties.
        /// </summary>
        /// <typeparam name="T">Type of objects to return.</typeparam>
        /// <param name="rowcount">Up to number of rows to return.</param>
        /// <returns>An enumeration of T objects.</returns>
        public IEnumerable<T> Take<T>(int rowcount)
            where T : new()
        {
            // Map column names to matching property info's:
            var typeProperties = typeof(T).GetProperties().ToDictionary(p => p.Name, p => p);
            var colProperties = new PropertyInfo[this.Reader.FieldCount];
            var colIndexer = new object[this.Reader.FieldCount][];
            for (int c = 0; c < colProperties.Length; c++)
            {
                PropertyInfo property;
                var columnName = Reader.GetName(c);
                var columnNameIsNumeric = numerical.IsMatch(columnName);
                var propertyName = (columnNameIsNumeric ? "Item" : columnName); // Use default index property "Item" if column name is numerical...
                if (typeProperties.TryGetValue(propertyName, out property))
                {
                    if (property.CanWrite)
                        colProperties[c] = property;
                    if (columnNameIsNumeric)
                        colIndexer[c] = new object[1] { Int32.Parse(columnName) };
                }
            }

            // Map rows to objects:
            for (int r = 0; r < rowcount; r++)
            {
                if (!Reader.Read()) break;

                var obj = new T();
                for (int c = 0; c < colProperties.Length; c++)
                {
                    var prop = colProperties[c];
                    if (prop != null)
                    {
                        if (!Reader.IsDBNull(c))
                        {
                            prop.SetValue(obj, Reader.GetValue(c), colIndexer[c]);
                        }
                    }
                }
                yield return obj;
            }
        }

        public virtual void Dispose()
        {
            if (this.reader != null) this.Reader.Dispose();
            this.Command.Dispose();
        }
    }
}
