using System;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Sp.Data;

namespace Sp.Sync.Data.Server
{
    /// <summary>
    /// Abstracts a sharepoint server synchronization provider that
    /// communicates with a SharePoint server and shields the client from
    /// the specific implementation of the server
    /// </summary>
    /// <remarks>
    /// The principal activities of the server synchronization are:
    /// * get the changes from the sharepoint server using the build-in change tracking of sharepoint
    /// * applies the incremental changes to the server
    /// * transforms the lists of sharepoint to relational tables
    /// </remarks>
    public class SpServerSyncProvider : ServerSyncProvider, IDisposable
    {
        /// <summary>
        /// Occurs before all changes to be applied to the client for a synchronization group are selected from the server.
        /// </summary>
        public event EventHandler<SelectingChangesEventArgs> SelectingChanges;
        
        /// <summary>
        /// Occurs after all changes to be applied to the client for a synchronization group are selected from the server.
        /// </summary>
        public event EventHandler<ChangesSelectedEventArgs> ChangesSelected;
        
        /// <summary>
        /// Occurs before changes are applied at the server for a synchronization group.
        /// </summary>
        public event EventHandler<ApplyingChangesEventArgs> ApplyingChanges;
        
        /// <summary>
        /// Occurs after a row fails to be applied at the server.
        /// </summary>
        public event EventHandler<ApplyChangeFailedEventArgs> ApplyChangeFailed;
        
        /// <summary>
        /// Occurs after all changes are applied at the server for a synchronization group.
        /// </summary>
        public event EventHandler<ChangesAppliedEventArgs> ChangesApplied;
        
        /// <summary>
        /// Occurs during the selection and application of changes for a synchronization group at the server.
        /// </summary>
        public event EventHandler<SyncProgressEventArgs> SyncProgress;

        /// <summary>
        /// Gets or sets the batch size (in rows) that is used by commands that retrieve changes from the server database. 
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets a SyncSchema object that contains information about the table schema on the server. 
        /// </summary>
        public SyncSchema Schema { get; set; }

        private SyncSchema CacheSchema { get; set; }

        /// <summary>
        /// Gets the collection of the sharepoint synchronization adapters
        /// </summary>
        public SpSyncAdapterCollection SyncAdapters { get; private set; }

        /// <summary>
        /// Gets the sharepoint connection
        /// </summary>
        public SpConnection Connection { get; private set;  }
        
        /// <summary>
        /// Initializes a new instance of the SpServerSyncProvider class.
        /// </summary>
        public SpServerSyncProvider(string connectionString)
        {
            SyncAdapters = new SpSyncAdapterCollection();
            Connection = new SpConnection(connectionString);
        }

        public SpServerSyncProvider(string server, string username, string password, string domain)
        {
            SyncAdapters = new SpSyncAdapterCollection();
            Connection = new SpConnection(server, username, password, domain);
        }

        /// <summary>
        /// Releases all resources used by the SpServerSyncProvider. 
        /// </summary>
        public override void Dispose()
        {
            Connection.Dispose();
        }

        /// <summary>
        /// Applies inserts, updates, and deletes for a synchronization group to the server database. 
        /// </summary>
        /// <param name="groupMetadata">A SyncGroupMetadata object that contains metadata about the synchronization group.</param>
        /// <param name="dataSet">A DataSet object that contains the changes to be applied to the server database for each table in the synchronization group.</param>
        /// <param name="syncSession">A SyncSession object that contains synchronization session variables, such as the ID of the client that is synchronizing.</param>
        /// <returns>A SyncContext object that contains synchronization data and metadata.</returns>
        public override SyncContext ApplyChanges(SyncGroupMetadata groupMetadata, DataSet dataSet, SyncSession syncSession)
        {
            if (groupMetadata == null)
                throw new ArgumentNullException("groupMetadata");
            if (syncSession == null)
                throw new ArgumentNullException("syncSession");

            SyncContext syncContext = new SyncContext();
            groupMetadata = InitializeMetadata(groupMetadata, dataSet, syncContext);

            // connect to database
            Connection.Open();

            // create transaction

            ApplyingChangesEventArgs applyingArgs = new ApplyingChangesEventArgs(groupMetadata, dataSet, syncSession, syncContext, Connection, null);
            OnApplyingChanges(applyingArgs);

            ApplyChangesInternal(groupMetadata, dataSet, syncSession, syncContext);
            // commit transaction

            ChangesAppliedEventArgs appliedArgs = new ChangesAppliedEventArgs(groupMetadata, syncSession, syncContext, Connection, null);
            OnChangesApplied(appliedArgs);

            // disconnect from database
            Connection.Close();

            return syncContext;
        }

        private void ApplyChangesInternal(SyncGroupMetadata groupMetadata, DataSet dataSet, SyncSession syncSession, SyncContext syncContext)
        {
            SyncStage syncStage = SyncStage.UploadingChanges;

            foreach (SyncTableMetadata tableMetadata in groupMetadata.TablesMetadata)
            {
                SpSyncAdapter adapter = null;

                if (this.SyncAdapters.Contains(tableMetadata.TableName))
                    adapter = this.SyncAdapters[tableMetadata.TableName];

                if (adapter == null)
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                        Messages.InvalidTableName, tableMetadata.TableName));


                // SpSyncAnchor anchor 
                if (!dataSet.Tables.Contains(tableMetadata.TableName))
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                        Messages.TableNotInSchema, tableMetadata.TableName));

                SyncTableProgress tableProgress = syncContext.GroupProgress.FindTableProgress(tableMetadata.TableName);

                DataTable dataTable = dataSet.Tables[tableMetadata.TableName];

                try
                {
                    Collection<SyncConflict> conflicts;
                    
                    adapter.Update(dataTable, Connection, out conflicts);

                    if (conflicts != null)
                    {
                        foreach (SyncConflict conflict in conflicts)
                        {
                            ApplyChangeFailedEventArgs failureArgs = new ApplyChangeFailedEventArgs(tableMetadata, conflict, null, syncSession, syncContext, Connection, null);
                            OnApplyChangeFailed(failureArgs);

                            if (failureArgs.Action == ApplyAction.Continue)
                            {
                                if (conflict != null)
                                {
                                    tableProgress.ChangesFailed++;
                                    tableProgress.Conflicts.Add(conflict);
                                }
                            }
                        }
                    }
                    tableProgress.ChangesApplied = dataTable.Rows.Count - tableProgress.ChangesFailed;
                }
                catch (Exception e)
                {
                    // handle errors?
                    if (SyncTracer.IsErrorEnabled())
                        SyncTracer.Error(e.ToString());
                }

                SyncProgressEventArgs args = new SyncProgressEventArgs(tableMetadata, tableProgress, groupMetadata, syncContext.GroupProgress, syncStage);
                OnSyncProgress(args);
            }
        }
        
        /// <summary>
        /// Selects for a table in the server database the inserts, updates, and deletes to apply to the client database for a synchronization group. 
        /// </summary>
        /// <param name="groupMetadata">A SyncGroupMetadata object that contains metadata about the synchronization group.</param>
        /// <param name="syncSession">A SyncSession object that contains synchronization session variables, such as the ID of the client that is synchronizing.</param>
        /// <returns>A SyncContext object that contains synchronization data and metadata.</returns>
        public override SyncContext GetChanges(SyncGroupMetadata groupMetadata, SyncSession syncSession)
        {
            if (groupMetadata == null)
                throw new ArgumentNullException("groupMetadata");
            if (syncSession == null)
                throw new ArgumentNullException("syncSession");

            SyncContext syncContext = new SyncContext();
            DataSet dataSet = new DataSet();
            groupMetadata = InitializeMetadata(groupMetadata, dataSet, syncContext);

            SelectingChangesEventArgs selectingArgs = new SelectingChangesEventArgs(groupMetadata, syncSession, syncContext, Connection, null);
            OnSelectingChanges(selectingArgs);
            
            SyncSchema schema = new SyncSchema();
            Collection<string> tables = new Collection<string>();
            Collection<string> missingTables = new Collection<string>();

            foreach (var tableMetadata in groupMetadata.TablesMetadata)
                tables.Add(tableMetadata.TableName);

            if (tables.Count > 0)
                schema = GetSchemaInternal(tables, out missingTables);

            if (missingTables != null)
            {
                string[] tableArray = new string[missingTables.Count];
                missingTables.CopyTo(tableArray, 0);
                SchemaException e = new SchemaException(String.Format(CultureInfo.CurrentCulture,
                    Messages.MissingTables, String.Join(", ", tableArray)));
                e.SyncStage = SyncStage.ReadingSchema;
                e.ErrorNumber = SyncErrorNumber.MissingTableSchema;
                throw e;
            }

            // FIX: Get schema from somewhere (possibly the adapter or a temporary schema)
            // Possible performance hit when in mobile

            EnumerateChanges(groupMetadata, syncSession, syncContext, schema);

            ChangesSelectedEventArgs selectedArgs = new ChangesSelectedEventArgs(groupMetadata, syncSession, syncContext, Connection, null);
            OnChangesSelected(selectedArgs);

            return syncContext;
        }

        /// <summary>
        /// Enumerates the changes from the server and puts them to the synccontext object
        /// </summary>
        /// <param name="groupMetadata">the metadata about the synchronization tables</param>
        /// <param name="syncSession">the object that contains synchronization variables</param>
        /// <param name="syncContext">the synchronization context to be changed</param>
        /// <param name="schema">the schema of the synchronization tables</param>
        private void EnumerateChanges(SyncGroupMetadata groupMetadata, SyncSession syncSession, SyncContext syncContext, SyncSchema schema)
        {
            SyncStage syncStage = SyncStage.DownloadingChanges;

            SpSyncGroupAnchor newSyncAnchor = new SpSyncGroupAnchor();

            bool hasMoreData = false;

            foreach (SyncTableMetadata tableMetadata in groupMetadata.TablesMetadata)
            {
                SpSyncAdapter adapter = null;

                if (this.SyncAdapters.Contains(tableMetadata.TableName))
                    adapter = this.SyncAdapters[tableMetadata.TableName];

                if (adapter == null)
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, 
                        Messages.InvalidTableName, tableMetadata.TableName));
               
                if (!schema.SchemaDataSet.Tables.Contains(tableMetadata.TableName))
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, 
                        Messages.TableNotInSchema, tableMetadata.TableName));

                DataTable dataTable = schema.SchemaDataSet.Tables[tableMetadata.TableName].Clone();

                SyncTableProgress tableProgress = syncContext.GroupProgress.FindTableProgress(tableMetadata.TableName);

                DataTable insertTable = dataTable.Clone();
                DataTable updateTable = dataTable.Clone();
                DataTable deleteTable = dataTable.Clone();

                SpSyncAnchor tableAnchor = SpSyncAnchor.Empty;

                if (tableMetadata.LastReceivedAnchor != null && 
                    tableMetadata.LastReceivedAnchor.Anchor != null)
                {
                    SpSyncGroupAnchor anchors = SpSyncGroupAnchor.Deserialize(tableMetadata.LastReceivedAnchor.Anchor);
                    if (anchors != null)
                    {
                        if (anchors.Contains(tableMetadata.TableName))
                            tableAnchor = anchors[tableMetadata.TableName];
                        else
                            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, 
                                Messages.AnchorNotValidForTable, tableMetadata.TableName));
                    }
                }

                SpSyncAnchor newAnchor = SpSyncAnchor.Empty;
                
                try
                {
                    newAnchor = adapter.SelectIncremental(tableAnchor, BatchSize, Connection, insertTable, updateTable, deleteTable);

                    hasMoreData = hasMoreData || newAnchor.PagingToken != null;

                    foreach (DataRow row in insertTable.Rows)
                    {
                        row.SetAdded();
                        dataTable.ImportRow(row);
                        tableProgress.Inserts++;
                    }

                    foreach (DataRow row in deleteTable.Rows)
                    {
                        row.Delete();
                        dataTable.ImportRow(row);
                        tableProgress.Deletes++;
                    }

                    foreach (DataRow row in updateTable.Rows)
                    {
                        row.SetModified();
                        dataTable.ImportRow(row);
                        tableProgress.Updates++;
                    }

                    if (syncContext.DataSet.Tables.Contains(tableMetadata.TableName))
                    {
                        DataTable contextTable = syncContext.DataSet.Tables[tableMetadata.TableName];
                        foreach (DataRow row in dataTable.Rows)
                            contextTable.ImportRow(row);
                    }
                    else
                    {
                        dataTable.TableName = tableMetadata.TableName;
                        syncContext.DataSet.Tables.Add(dataTable);
                    }
                }
                catch (Exception e)
                {
                    var e2 = new Microsoft.Synchronization.SyncException("", e);
                    throw e2;
                }
                finally 
                {
                    newSyncAnchor[tableMetadata.TableName] = newAnchor;
                }

                tableProgress.DataTable = dataTable;
                SyncProgressEventArgs args = new SyncProgressEventArgs(tableMetadata,tableProgress,groupMetadata, syncContext.GroupProgress, syncStage);
                OnSyncProgress(args);
                tableProgress.DataTable = null;
            }

            syncContext.NewAnchor = new SyncAnchor();

            syncContext.NewAnchor.Anchor = SpSyncGroupAnchor.Serialize(newSyncAnchor);

            int batchCount = groupMetadata.BatchCount == 0 ? 1 : groupMetadata.BatchCount;

            if (hasMoreData)
                syncContext.BatchCount = batchCount + 1;
            else
                syncContext.BatchCount = batchCount;
           
        }

        /// <summary>
        /// Returns a SyncSchema object that contains the schema for each table specified. 
        /// </summary>
        /// <param name="tableNames">A collection of table names for which the server provider should get the schema.</param>
        /// <param name="syncSession">A SyncSession object that contains synchronization session variables, such as the ID of the client that is synchronizing.</param>
        /// <returns>A SyncSchema object that contains the schema for each table that is specified.</returns>
        public override SyncSchema GetSchema(Collection<string> tableNames, SyncSession syncSession)
        {
            Collection<string> missingTables = null;

            SyncSchema schema = GetSchemaInternal(tableNames, out missingTables);

            if (missingTables != null)
            {
                string[] tableArray = new string[missingTables.Count];
                missingTables.CopyTo(tableArray, 0);
                SchemaException e = new SchemaException(String.Format(CultureInfo.CurrentCulture, 
                    Messages.MissingTables, String.Join(", ", tableArray)));
                e.SyncStage = SyncStage.ReadingSchema;
                e.ErrorNumber = SyncErrorNumber.MissingTableSchema;
                throw e;
            }

            return schema;
        }

        /// <summary>
        /// Returns a SyncServerInfo object that contains the table collection available from the server
        /// </summary>
        /// <param name="syncSession">A SyncSession object that contains synchronization session variables</param>
        /// <returns>A SyncServerInfo object that contains the collection of the tables that can be synchronized</returns>
        public override SyncServerInfo GetServerInfo(SyncSession syncSession)
        {
            Collection<SyncTableInfo> syncTableInfos = new Collection<SyncTableInfo>();

            foreach (SpSyncAdapter adapter in SyncAdapters)
                syncTableInfos.Add(new SyncTableInfo(adapter.TableName, adapter.Description));

            return new SyncServerInfo(syncTableInfos);
        }

        #region Event Handlers
        /// <summary>
        /// Raises the SyncProgress event. 
        /// </summary>
        /// <param name="args">A SyncProgressEventArgs object that contains event data.</param>
        protected virtual void OnSyncProgress(SyncProgressEventArgs args)
        {
            if (SyncProgress != null)
                SyncProgress(this, args);
        }

        protected virtual void OnApplyChangeFailed(ApplyChangeFailedEventArgs args)
        {
            if (ApplyChangeFailed != null)
                ApplyChangeFailed(this, args);
        }

        protected virtual void OnApplyingChanges(ApplyingChangesEventArgs args)
        {
            if (ApplyingChanges != null)
                ApplyingChanges(this, args);
        }

        protected virtual void OnChangesApplied(ChangesAppliedEventArgs args)
        {
            if (ChangesApplied != null)
                ChangesApplied(this, args);
        }

        protected virtual void OnChangesSelected(ChangesSelectedEventArgs args)
        {
            if (ChangesSelected != null)
                ChangesSelected(this, args);
        }

        protected virtual void OnSelectingChanges(SelectingChangesEventArgs args)
        {
            if (SelectingChanges != null)
                SelectingChanges(this, args);
        }

        #endregion 

        protected SyncSchema GetSchemaInternal(Collection<string> tableNames, out Collection<string> missingTables)
        {
            SyncSchema schema = null;
            Collection<string> missingTables2 = null;
            
            missingTables = new Collection<string>();

            /*
            if (CacheSchema != null)
            {
                return CacheSchema;
            }
            */

            if (Schema != null)
            {
                schema = GetSchemaFromSchemaDataset(tableNames, out missingTables2);

                if (missingTables2 != null)
                {
                    SyncSchema schema2 = GetSchemaFromDatabase(missingTables2, out missingTables);
                    schema.Merge(schema2);
                }
            }
            else
            {
                schema = GetSchemaFromDatabase(tableNames, out missingTables);
            }

            //CacheSchema = schema;

            return schema;
        }

        /// <summary>
        /// Gets a SyncSchema object created from the existing DataSet defined by the user
        /// </summary>
        /// <param name="tableNames">the names of the tables to be included in the schema</param>
        /// <param name="missingTables">the names of the missing tables not found in dataset</param>
        /// <returns>the SyncSchema object</returns>
        protected virtual SyncSchema GetSchemaFromSchemaDataset(Collection<string> tableNames, out Collection<string> missingTables)
        {
            SyncSchema schema = new SyncSchema();
            missingTables = new Collection<string>();

            foreach (string tableName in tableNames)
            {
                if (!Schema.Tables.Contains(tableName))
                    missingTables.Add(tableName);
            }

            foreach (DataTable table in Schema.SchemaDataSet.Tables)
            { 
                if (tableNames.Contains(table.TableName))
                {
                    DataTable table2 = table.Copy();
                    schema.SchemaDataSet.Tables.Add(table2);
                }
            }

            foreach (DataRelation relation in Schema.SchemaDataSet.Relations)
            {
                if (tableNames.Contains(relation.ParentTable.TableName)
                    && tableNames.Contains(relation.ChildTable.TableName))
                {
                    Collection<DataColumn> parentCollection = new Collection<DataColumn>();
                    foreach (DataColumn c in relation.ParentColumns)
                    {
                        parentCollection.Add(schema.SchemaDataSet.Tables[relation.ParentTable.TableName].Columns[c.ColumnName]);
                    }

                    Collection<DataColumn> childCollection = new Collection<DataColumn>();
                    foreach (DataColumn c in relation.ParentColumns)
                    {
                        childCollection.Add(schema.SchemaDataSet.Tables[relation.ChildTable.TableName].Columns[c.ColumnName]);
                    }

                    DataColumn[] parentArray = new DataColumn[parentCollection.Count];
                    DataColumn[] childArray  = new DataColumn[childCollection.Count];
                    parentCollection.CopyTo(parentArray, 0);
                    childCollection.CopyTo(childArray, 0);
                    DataRelation relation2 = new DataRelation(relation.RelationName, parentArray, childArray);
                    schema.SchemaDataSet.Relations.Add(relation2);
                }

            }

            if (missingTables.Count == 0)
                missingTables = null;

            return schema;         

        }

        /// <summary>
        /// Gets a SyncSchema object created by consulting the database, namely by filling the datatables from the SyncAdapters
        /// </summary>
        /// <param name="tableNames">the names of the tables to be included in the schema</param>
        /// <param name="missingTables">the names of the missing tables not found in the set of the SyncAdapters</param>
        /// <returns>the SyncSchema object</returns>
        protected virtual SyncSchema GetSchemaFromDatabase(Collection<string> tableNames, out Collection<string> missingTables)
        {
            SyncSchema schema = new SyncSchema();
            schema.SchemaDataSet = new DataSet();
            schema.SchemaDataSet.Locale = CultureInfo.InvariantCulture;
            missingTables = new Collection<string>();

            Connection.Open();

            foreach (string tableName in tableNames)
            {
                SpSyncAdapter adapter = null;
                if (SyncAdapters.Contains(tableName))
                {
                    adapter = SyncAdapters[tableName];
                }

                if (adapter != null)
                {
                    DataTable dataTable = null;

                    try
                    {
                        dataTable = adapter.FillSchema(dataTable, Connection);
                        dataTable.TableName = tableName;
                        schema.SchemaDataSet.Tables.Add(dataTable);
                    }
                    catch (Exception e)
                    {
                        missingTables.Add(tableName);
                        if (SyncTracer.IsErrorEnabled())
                            SyncTracer.Error(e.ToString());
                    }
                }
                else
                {
                    missingTables.Add(tableName);
                }
            }

            Connection.Close();

            if (missingTables.Count == 0)
                missingTables = null;

            return schema;
        }

        private SyncGroupMetadata InitializeMetadata(SyncGroupMetadata groupMetadata, DataSet dataSet, SyncContext syncContext)
        {
            syncContext.DataSet = dataSet;
            groupMetadata = GenerateOrderGroupMetadata(groupMetadata);
            syncContext.GroupProgress = new SyncGroupProgress(groupMetadata, dataSet);
            return groupMetadata;
        }

        private SyncGroupMetadata GenerateOrderGroupMetadata(SyncGroupMetadata groupMetadata)
        {
            if (groupMetadata.TablesMetadata.Count <= 1)
                return groupMetadata;

            SyncTableMetadata[] tableMetadatas = new SyncTableMetadata[groupMetadata.TablesMetadata.Count];
            for (int i = 0; i < groupMetadata.TablesMetadata.Count; ++i)
            { 
                int index = SyncAdapters.IndexOf(groupMetadata.TablesMetadata[i].TableName);

                if (index == -1)
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, 
                        Messages.InvalidTableName, groupMetadata.TablesMetadata[i].TableName));
                else
                    tableMetadatas[index] = groupMetadata.TablesMetadata[i];
            }
            SyncGroupMetadata newGroupMetadata = new SyncGroupMetadata(groupMetadata);
            newGroupMetadata.TablesMetadata.Clear();
            
            for (int i = 0; i < tableMetadatas.Length; i++)
            {
                if (tableMetadatas[i] != null)
                    newGroupMetadata.TablesMetadata.Add(tableMetadatas[i]);
            }

            return newGroupMetadata;
        }

    }
}
