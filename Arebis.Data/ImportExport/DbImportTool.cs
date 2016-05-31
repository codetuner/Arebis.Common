using Arebis.Data.Model;
using Arebis.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.ImportExport
{
    public class DbImportTool
    {
        [Arebis.Source.CodeToDo("Include rows with non-AutoIncrement Primary Keys and multi-column Primary Keys.")]
        public DbImportTool Import(ISimpleTaskHost host, DataSet data, DbConnection connection, DbTransaction transaction)
        {
            // Collections of actions:
            var toImport = new Queue<DependentRow>();
            var toImportPerTable = new Dictionary<string, List<DependentRow>>();
            var toUpdate = new Queue<DependentUpdate>();
            var sqlUpdateCmds = new Dictionary<string, DbCommand>();
            sqlUpdateCmds["IDENTITY"] = connection.CreateCommand("SELECT @@IDENTITY", CommandType.Text, transaction);

            // Change identity column values to negative to avoid conflicts on destination database:
            foreach (DataTable table in data.Tables)
            {
                foreach (DataColumn col in table.Columns)
                {
                    if (col.AutoIncrement)
                    {
                        col.ReadOnly = false;
                        foreach (DataRow row in table.Rows)
                        {
                            row[col] = -(int)row[col];
                        }
                    }
                }
            }

            // Read all rows:
            foreach (DataTable table in data.Tables)
            {
                var perTable = toImportPerTable[table.TableName] = new List<DependentRow>();
                foreach (DataRow row in table.Rows)
                {
                    var drow = new DependentRow() { Row = row };
                    toImport.Enqueue(drow);
                    perTable.Add(drow);
                }
            }

            host.Out.WriteLine("Importing {0} data rows...", toImport.Count);

            // Connect row dependencies:
            foreach (DataRelation relation in data.Relations)
            {
                // TODO: Should also include rows with non-AutoIncrement Primary Keys and multi-column Primary Keys.

                // Ignore relations where primary key is not an AutoIncrement field:
                if (relation.ParentColumns.Count() != 1 || !relation.ParentColumns[0].AutoIncrement) continue;

                var parentColumn = relation.ParentColumns[0];
                var childColumn = relation.ChildColumns[0];

                if (childColumn.AllowDBNull)
                {
                    // If foreign key is nullable, make it null and register a delayed update:
                    foreach (var parentRow in toImportPerTable[relation.ParentTable.TableName])
                    {
                        var parentPkValue = parentRow.Row[parentColumn];
                        foreach (var childRow in toImportPerTable[relation.ChildTable.TableName].Where(dr => Object.Equals(dr.Row[childColumn], parentPkValue)))
                        {
                            toUpdate.Enqueue(new DependentUpdate() { Row = childRow.Row, Column = childColumn, SourceRow = parentRow.Row, SourceColumn = parentColumn });
                            childRow.Row[childColumn] = DBNull.Value;
                        }
                    }
                }
                else
                {
                    // If foreign key row is not nullable, register dependency:
                    foreach (var parentRow in toImportPerTable[relation.ParentTable.TableName])
                    {
                        var parentPkValue = parentRow.Row[parentColumn];
                        foreach (var childRow in toImportPerTable[relation.ChildTable.TableName].Where(dr => Object.Equals(dr.Row[childColumn], parentPkValue)))
                        {
                            parentRow.AddChild(childRow);
                        }
                    }
                }
            }

            var totalActions = (double)(toImport.Count + toUpdate.Count);
            
            // Import rows:
            while (toImport.Count > 0)
            {
                var drow = toImport.Dequeue();
                if (drow.HasAllDependenciesResolved)
                {
                    drow.WriteToDatabase(host, connection, transaction, sqlUpdateCmds);
                    drow.MarkResolved();
                }
                else
                {
                    //Console.WriteLine("SKIPPED : {0} ({1})", drow.Row.Table.TableName, String.Join(", ", drow.Values().Take(1).Select(v => (v ?? "##null").ToString())));
                    toImport.Enqueue(drow);
                }

                host.ReportProgress((totalActions - toImport.Count - toUpdate.Count) / totalActions, drow.Row.Table.TableName);
            }

            // Dispose commands:
            sqlUpdateCmds.Values.ToList().ForEach(cmd => cmd.Dispose());

            host.Out.WriteLine("\r\nUpdating {0} data rows...", toUpdate.Count);

            // Perform delayed updates:
            using(var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;

                cmd.AddParameter("@SourceValue", null);
                cmd.AddParameter("@PkValue", null);
                
                while (toUpdate.Count > 0)
                {
                    var dupdate = toUpdate.Dequeue();
                    dupdate.WriteToDatabase(host, connection, cmd);

                    host.ReportProgress((totalActions - toImport.Count - toUpdate.Count) / totalActions, dupdate.Row.Table.TableName);
                }
            }

            host.Out.WriteLine();

            return this;
        }

        private class DependentRow
        {
            /// <summary>
            /// The current row.
            /// </summary>
            public DataRow Row;

            /// <summary>
            /// The rows this row depends on. The rows this row has foreign keys to.
            /// </summary>
            public HashSet<DependentRow> ParentRows = new HashSet<DependentRow>();

            /// <summary>
            /// The rows that are dependent on this row. The rows having foreign keys pointing to the primary key of this row.
            /// </summary>
            public HashSet<DependentRow> ChildRows = new HashSet<DependentRow>();

            /// <summary>
            /// Register a dependency for this row.
            /// </summary>
            public void AddChild(DependentRow childRow)
            {
                this.ChildRows.Add(childRow);
                childRow.ParentRows.Add(this);
            }

            public void MarkResolved()
            {
                foreach (var crow in ChildRows)
                {
                    crow.ParentRows.Remove(this);
                }

                this.ChildRows.Clear();
            }

            public bool HasAllDependenciesResolved
            {
                get
                {
                    return (ParentRows.Count == 0);
                }
            }

            public object[] Values()
            {
                var values = new object[Row.Table.Columns.Count];
                for (int i = 0; i < values.Length; i++)
                    values[i] = Row[i];

                return values;
            }

            internal void WriteToDatabase(ISimpleTaskHost host, DbConnection connection, DbTransaction transaction, Dictionary<string, DbCommand> sqlUpdateCmds)
            {
                //Console.WriteLine("          {0} ({1})", this.Row.Table.TableName, String.Join(", ", this.Values().Take(1).Select(v => (v ?? "##null").ToString())));

                var cmd = (DbCommand)null;
                try
                {
                    // Retrieve the (cached) SQL command:
                    if (!sqlUpdateCmds.TryGetValue(this.Row.Table.TableName, out cmd))
                    {
                        #region Create Command
                        var sb = new StringBuilder();
                        sb.Append("INSERT INTO [");
                        sb.Append(this.Row.Table.TableName.Replace(".", "].["));
                        sb.Append("] (");
                        foreach (DataColumn col in this.Row.Table.Columns)
                        {
                            if (col.AutoIncrement) continue;
                            sb.Append("[");
                            sb.Append(col.ColumnName);
                            sb.Append("], ");
                        }
                        sb.Length -= 2;
                        sb.Append(") VALUES (");
                        foreach (DataColumn col in this.Row.Table.Columns)
                        {
                            if (col.AutoIncrement) continue;
                            sb.Append("@");
                            sb.Append(col.ColumnName);
                            sb.Append(", ");
                        }
                        sb.Length -= 2;
                        sb.Append(")");
                        cmd = connection.CreateCommand();
                        cmd.CommandText = sb.ToString();
                        cmd.CommandType = CommandType.Text;
                        cmd.Transaction = transaction;
                        foreach (DataColumn col in this.Row.Table.Columns)
                        {
                            if (col.AutoIncrement) continue;
                            cmd.AddParameter("@" + col.ColumnName, null, ParameterDirection.Input, col.DataType.ToDbType());
                        }
                        #endregion
                        Debug.WriteLine(cmd.CommandText);
                        sqlUpdateCmds[this.Row.Table.TableName] = cmd;
                    }

                    // Set parameter values:
                    var pindex = 0;
                    foreach (DataColumn col in this.Row.Table.Columns)
                    {
                        if (col.AutoIncrement) continue;
                        cmd.Parameters[pindex++].Value = Row[col];
                    }

                    // Execute:
                    cmd.ExecuteNonQuery();

                }
                catch
                {
                    // Provide details about the failing row:
                    var sb = new StringBuilder();
                    sb.AppendLine("Failed to insert following row:");
                    sb.AppendLine(cmd.CommandText);
                    foreach (DbParameter param in cmd.Parameters)
                    {
                        sb.AppendFormat("- {0} = {1}", param.ParameterName, param.Value);
                        sb.AppendLine();
                    }
                    host.Error.Write(sb.ToString());

                    // Rethrow:
                    throw;
                }
                finally
                { }

                // Retrieve identity values:
                foreach (DataColumn col in this.Row.Table.Columns)
                {
                    if (col.AutoIncrement)
                    {
                        col.ReadOnly = false;
                        Row[col] = sqlUpdateCmds["IDENTITY"].ExecuteScalar();
                        break;
                    }
                }
            }
        }

        private class DependentUpdate
        {
            /// <summary>
            /// The current row to be updated.
            /// </summary>
            public DataRow Row;

            /// <summary>
            /// The column to update;
            /// </summary>
            public DataColumn Column;

            /// <summary>
            /// The row containing the value to use.
            /// </summary>
            public DataRow SourceRow;

            /// <summary>
            /// The column containing the value to use.
            /// </summary>
            public DataColumn SourceColumn;

            public void WriteToDatabase(ISimpleTaskHost host, DbConnection connection, DbCommand command)
            {
                DataColumn pk = Row.Table.PrimaryKey[0];
                //Console.WriteLine(String.Format("UPDATE {0} SET {1} = {2} WHERE {3} = {4}", Row.Table.TableName, Column.ColumnName, SourceRow[SourceColumn], pk.ColumnName, Row[pk]));

                command.CommandText = String.Format("UPDATE [{0}] SET [{1}] = @SourceValue WHERE [{2}] = @PkValue", Row.Table.TableName.Replace(".", "].["), Column.ColumnName, pk.ColumnName);
                command.Parameters["@SourceValue"].Value = SourceRow[SourceColumn];
                command.Parameters["@PkValue"].Value = Row[pk];
                Debug.WriteLine(command.CommandText);
                command.ExecuteNonQuery();
            }
        }
    }
}
