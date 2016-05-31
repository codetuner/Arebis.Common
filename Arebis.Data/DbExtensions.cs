using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data
{
    public static class DbExtensions
    {
        #region DbType Mapping

        private static Dictionary<Type, DbType> typeMap;

        static DbExtensions()
        {
            typeMap = new Dictionary<Type, DbType>();
            typeMap[typeof(byte)] = DbType.Byte;
            typeMap[typeof(sbyte)] = DbType.SByte;
            typeMap[typeof(short)] = DbType.Int16;
            typeMap[typeof(ushort)] = DbType.UInt16;
            typeMap[typeof(int)] = DbType.Int32;
            typeMap[typeof(uint)] = DbType.UInt32;
            typeMap[typeof(long)] = DbType.Int64;
            typeMap[typeof(ulong)] = DbType.UInt64;
            typeMap[typeof(float)] = DbType.Single;
            typeMap[typeof(double)] = DbType.Double;
            typeMap[typeof(decimal)] = DbType.Decimal;
            typeMap[typeof(bool)] = DbType.Boolean;
            typeMap[typeof(string)] = DbType.String;
            typeMap[typeof(char)] = DbType.StringFixedLength;
            typeMap[typeof(Guid)] = DbType.Guid;
            typeMap[typeof(DateTime)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            typeMap[typeof(byte[])] = DbType.Binary;
            typeMap[typeof(byte?)] = DbType.Byte;
            typeMap[typeof(sbyte?)] = DbType.SByte;
            typeMap[typeof(short?)] = DbType.Int16;
            typeMap[typeof(ushort?)] = DbType.UInt16;
            typeMap[typeof(int?)] = DbType.Int32;
            typeMap[typeof(uint?)] = DbType.UInt32;
            typeMap[typeof(long?)] = DbType.Int64;
            typeMap[typeof(ulong?)] = DbType.UInt64;
            typeMap[typeof(float?)] = DbType.Single;
            typeMap[typeof(double?)] = DbType.Double;
            typeMap[typeof(decimal?)] = DbType.Decimal;
            typeMap[typeof(bool?)] = DbType.Boolean;
            typeMap[typeof(char?)] = DbType.StringFixedLength;
            typeMap[typeof(Guid?)] = DbType.Guid;
            typeMap[typeof(DateTime?)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            //typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
        }

        #endregion

        /// <summary>
        /// Returns best match of DbType for given type.
        /// Returns DbType.Object if no match found.
        /// </summary>
        /// <remarks>
        /// Based on http://stackoverflow.com/questions/7952142/how-to-resolve-system-type-to-system-data-dbtype.
        /// </remarks>
        public static DbType ToDbType(this Type type)
        {
            DbType result;
            if (typeMap.TryGetValue(type, out result))
            {
                return result;
            }
            else
            {
                return DbType.Object;
            }
        }

        public static DbCommand AddParameter(this DbCommand cmd, string name, object value, ParameterDirection direction = ParameterDirection.Input, DbType dbType = DbType.String, int? size = null)
        {
            var param = cmd.CreateParameter();
            param.DbType = dbType;
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            param.Direction = direction;
            if (size.HasValue) param.Size = size.Value;
            cmd.Parameters.Add(param);
            return cmd;
        }

        public static DbCommand CreateCommand(this DbConnection conn, string commandText, CommandType commandType = CommandType.Text, DbTransaction transaction = null)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;
            cmd.Transaction = transaction;
            return cmd;
        }

        public static DbDataAdapter CreateDataAdapter(this DbConnection conn)
        {
            var factory = DbProviderFactories.GetFactory(conn);
            var da = factory.CreateDataAdapter();
            return da;
        }

        public static void FillDataSet(this DbConnection conn, DataSet dataSet, string tableName, string sql, MissingSchemaAction missingSchemaAction = MissingSchemaAction.Add)
        {
            using (var da = conn.CreateDataAdapter())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                da.MissingSchemaAction = missingSchemaAction;
                da.Fill(dataSet, tableName);
            }
        }

        /// <summary>
        /// Sets the isolation level of a SqlConnection.
        /// </summary>
        [Arebis.Source.CodeSource("http://blogs.u2u.be/diederik/post/2010/08/30/Getting-and-setting-the-Transaction-Isolation-Level-on-a-SQL-Entity-Connection.aspx")]
        public static void SetIsolationLevel(this System.Data.SqlClient.SqlConnection connection, IsolationLevel isolationLevel)
        {
            if (isolationLevel == IsolationLevel.Unspecified || isolationLevel == IsolationLevel.Chaos)
            {
                throw new Exception(string.Format("Isolation Level '{0}' can not be set.", isolationLevel.ToString()));
            }

            string isolationLevelSqlName;
            switch (isolationLevel)
            {
                case IsolationLevel.Chaos: isolationLevelSqlName = "CHAOS"; break;
                case IsolationLevel.ReadCommitted: isolationLevelSqlName = "READ COMMITTED"; break;
                case IsolationLevel.ReadUncommitted: isolationLevelSqlName = "READ UNCOMMITTED"; break;
                case IsolationLevel.RepeatableRead: isolationLevelSqlName = "REPEATABLE READ"; break;
                case IsolationLevel.Serializable: isolationLevelSqlName = "SERIALIZABLE"; break;
                case IsolationLevel.Snapshot: isolationLevelSqlName = "SNAPSHOT"; break;
                default: isolationLevelSqlName = "UNSPECIFIED"; break;
            }

            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SET TRANSACTION ISOLATION LEVEL " + isolationLevelSqlName;
            command.ExecuteNonQuery();
        }

        [Arebis.Source.CodeSource("http://blogs.u2u.be/diederik/post/2010/08/30/Getting-and-setting-the-Transaction-Isolation-Level-on-a-SQL-Entity-Connection.aspx")]
        public static IsolationLevel GetIsolationLevel(this System.Data.SqlClient.SqlConnection connection)
        {
            string query =
                @"SELECT CASE transaction_isolation_level
                    WHEN 0 THEN 'Unspecified'
                    WHEN 1 THEN 'ReadUncommitted'
                    WHEN 2 THEN 'ReadCommitted'
                    WHEN 3 THEN 'RepeatableRead'
                    WHEN 4 THEN 'Serializable'
                    WHEN 5 THEN 'Snapshot'
                    END AS [Transaction Isolation Level]
            FROM sys.dm_exec_sessions
            WHERE session_id = @@SPID";

            IDbCommand command = connection.CreateCommand();
            command.CommandText = query;
            string result = command.ExecuteScalar().ToString();

            return (IsolationLevel)Enum.Parse(typeof(IsolationLevel), result);
        }

        public static long? GetLastGeneratedIdentity(this System.Data.SqlClient.SqlConnection connection)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT SCOPE_IDENTITY()";
                return (long?)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Returns the value of the "SELECT @@IDENTITY" query.
        /// </summary>
        public static long? GetLastGeneratedIdentity(this System.Data.OleDb.OleDbConnection connection)
        {
            // Assume MS-Access:
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT @@IDENTITY";
                return (long?)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Executes the query, and returns the first row in the resultset returned by the query. Extra rows are ignored.
        /// Return null if no (first) row(s).
        /// </summary>
        public static object[] ExecuteSingleRow(this IDbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var result = new object[reader.FieldCount];
                    reader.GetValues(result);
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        public static int? GetInt32OrNull(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
                return null;
            else
                return reader.GetInt32(ordinal);
        }

        public static string GetStringOrNull(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
                return null;
            else
                return reader.GetString(ordinal);
        }

        public static DateTime? GetDateTimeOrNull(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
                return null;
            else
                return reader.GetDateTime(ordinal);
        }

        public static byte[] GetBytesOrNull(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
                return null;
            else
            {
                var len = reader.GetBytes(ordinal, 0, null, 0, 0);
                if (len > Int32.MaxValue) throw new Exception("Binary data too long for GetBytesOrNull extension method's implementation.");
                var buffer = new byte[len];
                reader.GetBytes(ordinal, 0, buffer, 0, (int)len);
                return buffer;
            }
        }

        public static string ToCsv(this DbDataReader reader, CsvGeneratorSettings settings = null)
        {
            return new CsvGenerator(settings).Generate(reader);
        }

        public static string ToCsv(this DataTable table, CsvGeneratorSettings settings = null)
        {
            using (var reader = table.CreateDataReader())
            {
                return new CsvGenerator(settings).Generate(reader);
            }
        }

        /// <summary>
        /// Returns DataTable rows where the given columns have the given values. Only exact matches are returned.
        /// </summary>
        public static IEnumerable<DataRow> WhereValues(this DataTable dataTable, DataColumn[] columns, object[] values)
        {
            if (dataTable == null) throw new ArgumentNullException("dataTable");
            if (columns == null) throw new ArgumentNullException("columns");
            if (values == null) throw new ArgumentNullException("values");
            if (columns.Length != values.Length) throw new ArgumentException("Columns and values arrays must have the same length.");

            var length = columns.Length;
            foreach (DataRow row in dataTable.Rows)
            {
                var isMatch = true;
                for (int i = 0; i < length; i++)
                {
                    if (!Object.Equals(row[i], values[i])) { isMatch = false; break; }
                }

                if (isMatch) yield return row;
            }
        }

        /// <summary>
        /// Returns an array of values, one value for each given column of the given row.
        /// </summary>
        public static object[] ValuesFor(this DataRow row, DataColumn[] columns)
        {
            var values = new object[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                values[i] = row[columns[i]];
            }

            return values;
        }

        public static IEnumerable<Object[]> GetAllRows(this DbCommand command, bool includeHeaderRow)
        {
            using (var reader = command.ExecuteReader())
            {
                if (includeHeaderRow)
                {
                    var row = new Object[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader.GetName(i);
                    }
                    yield return row;
                }
                while (reader.Read())
                {
                    var row = new Object[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader.GetValue(i);
                    }
                    yield return row;
                }
            }
        }
    }
}
