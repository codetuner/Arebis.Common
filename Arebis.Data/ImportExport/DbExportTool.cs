using Arebis.Algorithms;
using Arebis.Data;
using Arebis.Data.Model;
using Arebis.Tasks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.ImportExport
{
    public class DbExportTool : IDisposable
    {
        private DatabaseModel databaseModel;
        private DataSet targetDataSet;

        #region Constructors and Dispose

        public DbExportTool(string connectionName)
            : this(ConfigurationManager.ConnectionStrings[connectionName])
        { }

        public DbExportTool(ConnectionStringSettings connectionStringSettings)
            : this(connectionStringSettings.ConnectionString, connectionStringSettings.ProviderName)
        { }

        public DbExportTool(string connectionString, string providerName)
            : this(CreateConnection(connectionString, providerName), CreateModelBuilder(providerName), true)
        { }

        public DbExportTool(System.Data.SqlClient.SqlConnection connection)
            : this(connection, new SqlModelBuilder(), false)
        { }

        public DbExportTool(DbConnection connection, IModelBuilder modelBuilder)
            : this(connection, modelBuilder, false)
        { }

        private DbExportTool(DbConnection connection, IModelBuilder modelBuilder, bool isConnectionOwner)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (modelBuilder == null) throw new ArgumentNullException("modelBuilder");

            this.IsConnectionOwner = isConnectionOwner;
            this.Connection = connection;
            this.ModelBuilder = modelBuilder;
            this.RelationsToExclude = new HashSet<ModelRelation>();
            this.RelationsToInclude = new HashSet<ModelRelation>();
            this.TableFilters = new Dictionary<ModelTable, string>();
            this.QueryArguments = new Dictionary<string, string>();
        }

        private static DbConnection CreateConnection(string connectionString, string providerName)
        {
            var factory = DbProviderFactories.GetFactory(providerName);
            var conn = factory.CreateConnection();
            conn.ConnectionString = connectionString;
            conn.Open();
            return conn;
        }

        private static IModelBuilder CreateModelBuilder(string providerName)
        {
            return ModelBuilders.GetModelBuilder(providerName);
        }

        public virtual void Dispose()
        {
            if (IsConnectionOwner) this.Connection.Dispose();
        }

        #endregion

        #region Run by DbExportArgs

        public static DataSet Run(ISimpleTaskHost host, DbExportArgs args)
        {
            var connection = args.FromConnection;
            using (var xtool = new DbExportTool(connection.ConnectionString, connection.ProviderName ?? "System.Data.SqlClient"))
            {
                xtool.ExportMode = args.ExportMode ?? DbExportMode.None;
                foreach (var item in args.RelationsToExclude)
                    xtool.ExcludeRelation(item);
                foreach (var item in args.RelationsToInclude)
                    xtool.IncludeRelation(item);
                foreach (var item in args.TableFilters)
                    xtool.AddTableFilter(item.TableName, item.WhereCondition);
                foreach (var item in args.QueryArguments)
                    xtool.QueryArguments[item.Key] = item.Value;
                foreach (var item in args.Queries)
                    xtool.Export(host, item.TableName, item.WhereCondition);
                return xtool.TargetDataSet;
            }
        }

        #endregion

        /// <summary>
        /// Whether this object is owner of the database connection and
        /// is responsible of releasing the connection.
        /// </summary>
        public bool IsConnectionOwner { get; private set; }

        /// <summary>
        /// The underlying database connection.
        /// </summary>
        public DbConnection Connection { get; private set; }

        /// <summary>
        /// The ModelBuilder instance to use to build the database model.
        /// </summary>
        public IModelBuilder ModelBuilder { get; private set; }

        /// <summary>
        /// A model of the database to perform export actions on.
        /// </summary>
        public DatabaseModel DatabaseModel
        {
            get
            {
                if (databaseModel == null)
                {
                    databaseModel = ModelBuilder.Build(this.Connection);
                }

                return databaseModel;
            }
        }

        /// <summary>
        /// Export modus.
        /// </summary>
        public DbExportMode ExportMode { get; set; }

        /// <summary>
        /// Relations to exclude.
        /// </summary>
        public HashSet<ModelRelation> RelationsToExclude { get; set; }

        /// <summary>
        /// Child to parent relations to include even if not in Full export mode.
        /// </summary>
        public HashSet<ModelRelation> RelationsToInclude { get; set; }

        /// <summary>
        /// Marks the given relation as to be included even when not in Full mode.
        /// By default, only parent to child relations are included. In Full mode,
        /// all relations are included.
        /// </summary>
        /// <param name="name">Name of the foreign key relationship</param>
        /// <returns>Supports fluent syntax.</returns>
        public DbExportTool IncludeRelation(string name)
        {
            try
            {
                RelationsToInclude.Add(DatabaseModel.GetRelation(name));
                return this;
            }
            catch
            {
                throw new ApplicationException(String.Format("Invalid relation name \"{0}\".", name));
            }
        }

        public DbExportTool ExcludeRelation(string name)
        {
            try
            {
                RelationsToExclude.Add(DatabaseModel.GetRelation(name));
                return this;
            }
            catch
            {
                throw new ApplicationException(String.Format("Invalid relation name \"{0}\".", name));
            }
        }

        /// <summary>
        /// Extra filters to apply when querying tables.
        /// </summary>
        public Dictionary<ModelTable, String> TableFilters { get; set; }

        /// <summary>
        /// Adds an extra filter to apply when querying the given table.
        /// </summary>
        /// <param name="tableName">Name of the table to apply the filtering on.</param>
        /// <param name="filterCondition">Filter expressed as an SQL where expression.</param>
        /// <returns>Supports fluent syntax.</returns>
        public DbExportTool AddTableFilter(string tableName, string filterCondition)
        {
            try
            {
                this.TableFilters[DatabaseModel.GetTableOrView(tableName)] = filterCondition;
                return this;
            }
            catch
            {
                throw new ApplicationException(String.Format("Invalid table name \"{0}\".", tableName));
            }
        }

        /// <summary>
        /// Arguments to pass when performing queries.
        /// </summary>
        public Dictionary<string, string> QueryArguments { get; set; }

        /// <summary>
        /// The dataset containing the exported data.
        /// </summary>
        public DataSet TargetDataSet
        {
            get
            {
                if (targetDataSet == null)
                {
                    targetDataSet = new DataSet(DatabaseModel.Name);
                }

                return targetDataSet;
            }
            set
            {
                targetDataSet = value;
            }
        }

        /// <summary>
        /// Exports records from the given table that match the given where condition do dataset.
        /// </summary>
        /// <param name="host">The host executing this task.</param>
        /// <param name="fromTable">Table to export rows from.</param>
        /// <param name="where">Where condition to match rows to export.</param>
        /// <param name="parameters">Optional parameters of the where clausule (named @p0, @p1, @p2,...).</param>
        /// <returns>Supports fluent syntax.</returns>
        public DbExportTool Export(ISimpleTaskHost host, string fromTable, string where, object[] parameters = null)
        {
            var relationsToAdd = new List<DataRelation>();
            
            this.Export(
                host,
                new ExportAction() { Table = DatabaseModel.GetTableOrView(fromTable), Where = where, Parameters = parameters },
                relationsToAdd);

            foreach (var relation in relationsToAdd)
            {
                try
                {
                    TargetDataSet.Relations.Add(relation);
                }
                catch (Exception ex)
                {
                    host.Error.WriteLine(String.Format("Warning: failed to rebuild relation {0} : {1}", relation.RelationName, ex.Message));
                }
            }

            return this;
        }

        /// <summary>
        /// Effective implementation of the Export.
        /// </summary>
        private void Export(ISimpleTaskHost host, ExportAction initialAction, List<DataRelation> relationsToAdd)
        {
            // Create action queue and seed with initialAction:
            var actionQueue = new ExportActionsQueue();
            actionQueue.Enqueue(initialAction);

            // Run over all actions in queue:
            while (actionQueue.Count > 0)
            {
                // Get next action:
                var action = actionQueue.Dequeue();

                // Host handling:
                if (host.IsAbortRequested) { host.Aborting(); return; }
                host.Out.WriteLine("  FROM {0} WHERE {1} [{2}]", action.Table, action.Where, (action.Parameters == null) ? "" : String.Join(", ", action.Parameters));
                
                // Retrieve or Create table definition on dataset:
                var dsTableName = action.Table.SchemaDotName;
                var dsTable = (DataTable)null;
                if (TargetDataSet.Tables.Contains(dsTableName))
                {
                    // Retrieve table:
                    dsTable = TargetDataSet.Tables[dsTableName];
                }
                else
                {
                    // Create table:
                    Connection.FillDataSet(TargetDataSet, dsTableName, "SELECT * FROM " + action.Table.ToString() + " WHERE (1=0)", MissingSchemaAction.AddWithKey);
                    dsTable = TargetDataSet.Tables[dsTableName];

                    // Create relational constraints to tables already existing, in one direction:
                    foreach (var relation in action.Table.GetFromRelations())
                    {
                        if (TargetDataSet.Tables.Contains(relation.ForeignTable.SchemaDotName))
                        {
                            var parentCols = new List<DataColumn>();
                            foreach (var col in relation.PrimaryColumns)
                            {
                                parentCols.Add(dsTable.Columns[col.Name]);
                            }

                            var childTable = TargetDataSet.Tables[relation.ForeignTable.SchemaDotName];
                            var childCols = new List<DataColumn>();
                            foreach (var col in relation.ForeignColumns)
                            {
                                childCols.Add(childTable.Columns[col.Name]);
                            }

                            relationsToAdd.Add(new DataRelation(relation.SchemaDotName, parentCols.ToArray(), childCols.ToArray()));
                        }
                    }
                    // and in the other direction:
                    foreach (var relation in action.Table.GetToRelations())
                    {
                        if (TargetDataSet.Tables.Contains(relation.PrimaryTable.SchemaDotName))
                        {
                            var parentTable = TargetDataSet.Tables[relation.PrimaryTable.SchemaDotName];
                            if (dsTable == parentTable) continue;
                            var parentCols = new List<DataColumn>();
                            foreach (var col in relation.PrimaryColumns)
                            {
                                parentCols.Add(parentTable.Columns[col.Name]);
                            }

                            var childCols = new List<DataColumn>();
                            foreach (var col in relation.ForeignColumns)
                            {
                                childCols.Add(dsTable.Columns[col.Name]);
                            }

                            relationsToAdd.Add(new DataRelation(relation.SchemaDotName, parentCols.ToArray(), childCols.ToArray()));
                        }
                    }
                }

                // Construct SQL query for export:
                var sql = "SELECT * FROM " + action.Table.ToString() + " WHERE (" + action.Where + ")";

                // Append filters (if any):
                foreach (var filter in this.TableFilters.Where(f => f.Key == action.Table))
                {
                    sql += " AND (" + filter.Value +")";
                }

                using (var cmd = Connection.CreateCommand(sql))
                {
                    // Add system query parameters (if any):
                    if (action.Parameters != null && action.Parameters.Length > 0)
                    {
                        for (int i = 0; i < action.Parameters.Length; i++)
                        {
                            cmd.AddParameter("@p" + i, action.Parameters[i]);
                        }
                    }

                    // Add user query parameters (if any) (only on root queries, where action.Parameters = null):
                    if (action.Parameters == null)
                    {
                        foreach (var item in this.QueryArguments)
                        {
                            if (!sql.Contains(item.Key)) continue; // Skip arguments that do not appear in sql.
                            cmd.AddParameter(item.Key, item.Value);
                        }
                    }

                    // Execute the query:
                    using (var reader = cmd.ExecuteReader())
                    {
                        // Construct a map of columns:
                        var map = (DataReaderMap)null;
                        map = new DataReaderMap(reader);

                        // Iterate over all returned rows:
                        while (reader.Read())
                        {
                            // Retrieve row values:
                            var values = new object[reader.FieldCount];
                            reader.GetValues(values);

                            try
                            {
                                // Add row to dataset:
                                var dataRow = TargetDataSet.Tables[dsTableName].Rows.Add(values);

                                // Enqueue related child rows for export (except if excluded):
                                foreach (var rel in action.Table.GetFromRelations())
                                {
                                    if ((this.ExportMode == DbExportMode.None) && (!this.RelationsToInclude.Contains(rel))) continue;
                                    if (this.RelationsToExclude.Contains(rel)) continue;

                                    var vs = new object[rel.ForeignColumns.Count];
                                    var wh = "(" + String.Join(") AND (", Enumerate.FromTo(0, rel.ForeignColumns.Count - 1).Select(i => "[" + rel.ForeignColumns[i].Name + "] = @p" + i).ToArray()) + ")";
                                    for (int i = 0; i < rel.ForeignColumns.Count; i++)
                                    {
                                        vs[i] = values[map.ColumnIndexes[rel.PrimaryColumns[i].Name]];
                                    }
                                    actionQueue.Enqueue(new ExportAction() { Table = rel.ForeignTable, Where = wh, Parameters = vs });
                                }

                                // Enqueue related parent rows for export (if in Full mode or relation explicitely included):
                                foreach (var rel in action.Table.GetToRelations())
                                {
                                    if ((this.ExportMode != DbExportMode.Full) && (!this.RelationsToInclude.Contains(rel))) continue;
                                    if (this.RelationsToExclude.Contains(rel)) continue;

                                    var vs = new object[rel.PrimaryColumns.Count];
                                    var wh = "(" + String.Join(") AND (", Enumerate.FromTo(0, rel.PrimaryColumns.Count - 1).Select(i => "[" + rel.PrimaryColumns[i].Name + "] = @p" + i).ToArray()) + ")";
                                    for (int i = 0; i < rel.PrimaryColumns.Count; i++)
                                    {
                                        vs[i] = values[map.ColumnIndexes[rel.ForeignColumns[i].Name]];
                                    }
                                    actionQueue.Enqueue(new ExportAction() { Table = rel.PrimaryTable, Where = wh, Parameters = vs });
                                }
                            }
                            catch (System.Data.ConstraintException)
                            {
                                // Ignore primary key violation: record was already present...
                            }
                        }
                    }
                }
            }
        }

        private class ExportAction
        {
            public ModelTable Table;

            public string Where;

            public object[] Parameters;

            public override bool Equals(object obj)
            {
                var other = obj as ExportAction;
                if (other == null) return false;
                if (Object.ReferenceEquals(this, other)) return true;
                if (!Object.ReferenceEquals(this.Table, other.Table))
                {
                    if (this.Table == null) return false;
                    if (!this.Table.Equals(other.Table)) return false;
                }
                if (!Object.ReferenceEquals(this.Where, other.Where))
                {
                    if (this.Where == null) return false;
                    if (!this.Where.Equals(other.Where)) return false;
                }
                if (!Object.ReferenceEquals(this.Parameters, other.Parameters))
                {
                    if (this.Parameters == null) return false;
                    if (other.Parameters == null) return false;
                    if (this.Parameters.Length != other.Parameters.Length) return false;
                    for(int i=0; i<this.Parameters.Length; i++)
                    {
                        if (!Object.ReferenceEquals(this.Parameters[i], other.Parameters[i]))
                        {
                            if (this.Parameters[i] == null) return false;
                            if (!this.Parameters[i].Equals(other.Parameters[i])) return false;
                        }
                    }
                }

                return true;
            }

            public override int GetHashCode()
            {
                var hc = this.GetType().GetHashCode();
                if (this.Table != null) hc ^= this.Table.GetHashCode();
                if (this.Where != null) hc ^= this.Where.GetHashCode();
                if (this.Parameters != null)
                {
                    foreach (var p in this.Parameters)
                    {
                        if (p != null) hc ^= p.GetHashCode();
                    }
                }

                return hc;
            }
        }

        private class ExportActionsQueue : Queue<ExportAction>
        {
            //private HashSet<ExportAction> history = new HashSet<ExportAction>();

            //public new void Enqueue(ExportAction item)
            //{
            //    // Enqueue only if was not queued earlier to avoid circularity:
            //    if (!history.Contains(item))
            //    {
            //        history.Add(item);
            //        base.Enqueue(item);
            //    }
            //}
        }
    }
}
