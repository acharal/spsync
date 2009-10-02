using System;
using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Synchronization.Data;
using System.Globalization;
using Microsoft.Synchronization;

namespace SpServerSync.Data
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
        public EventHandler<SelectingChangesEventArgs> SelectingChanges;
        
        /// <summary>
        /// Occurs after all changes to be applied to the client for a synchronization group are selected from the server.
        /// </summary>
        public EventHandler<ChangesSelectedEventArgs> ChangesSelected;
        
        /// <summary>
        /// Occurs before changes are applied at the server for a synchronization group.
        /// </summary>
        public EventHandler<ApplyingChangesEventArgs> ApplyingChanges;
        
        /// <summary>
        /// Occurs after a row fails to be applied at the server.
        /// </summary>
        public EventHandler<ApplyChangeFailedEventArgs> ApplyChangeFailed;
        
        /// <summary>
        /// Occurs after all changes are applied at the server for a synchronization group.
        /// </summary>
        public EventHandler<ChangesAppliedEventArgs> ChangesApplied;
        
        /// <summary>
        /// Occurs during the selection and application of changes for a synchronization group at the server.
        /// </summary>
        public EventHandler<SyncProgressEventArgs> SyncProgress;

        /// <summary>
        /// Gets or sets the batch size (in rows) that is used by commands that retrieve changes from the server database. 
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets a SyncSchema object that contains information about the table schema on the server. 
        /// </summary>
        public SyncSchema Schema { get; set; }

        /// <summary>
        /// Gets the collection of the synchronization adapters
        /// </summary>
        public SpSyncAdapterCollection SyncAdapters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the SpServerSyncProvider class.
        /// </summary>
        public SpServerSyncProvider()
        {
            SyncAdapters = new SpSyncAdapterCollection();
        }

        /// <summary>
        /// Releases all resources used by the SpServerSyncProvider. 
        /// </summary>
        public override void Dispose()
        {

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

            // connect to database
            

            // create transaction

            ApplyingChangesEventArgs applyingArgs = new ApplyingChangesEventArgs(groupMetadata, dataSet, syncSession, syncContext, null, null);
            OnApplyingChanges(applyingArgs);

            // apply deletes
            // apply inserts
            // apply updates

            // commit transaction

            ChangesAppliedEventArgs appliedArgs = new ChangesAppliedEventArgs(groupMetadata, syncSession, syncContext, null, null);
            OnChangesApplied(appliedArgs);

            // disconnect from database

            throw new NotImplementedException();
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

            SelectingChangesEventArgs selectingArgs = new SelectingChangesEventArgs(groupMetadata, syncSession, syncContext, null, null);
            OnSelectingChanges(selectingArgs);

            // enumerate changes insert
            // enumerate changes updates
            // enumerate changes deletes

            ChangesSelectedEventArgs selectedArgs = new ChangesSelectedEventArgs(groupMetadata, syncSession, syncContext, null, null);
            OnChangesSelected(selectedArgs);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a SyncSchema object that contains the schema for each table specified. 
        /// </summary>
        /// <param name="tableNames">A collection of table names for which the server provider should get the schema.</param>
        /// <param name="syncSession">A SyncSession object that contains synchronization session variables, such as the ID of the client that is synchronizing.</param>
        /// <returns>A SyncSchema object that contains the schema for each table that is specified.</returns>
        public override SyncSchema GetSchema(Collection<string> tableNames, SyncSession syncSession)
        {
            SyncSchema schema = null;
            Collection<string> missingTables = null;
            Collection<string> missingTables2 = new Collection<string>();
            
            if (Schema != null)
            {
                schema = GetSchemaFromSchemaDataset(tableNames, out missingTables);

                if (missingTables != null)
                {
                    SyncSchema schema2 = GetSchemaFromDatabase(missingTables, out missingTables2);
                    schema.Merge(schema2);
                }
            }
            else
            {
                schema = GetSchemaFromDatabase(tableNames, out missingTables);
            }

            if (missingTables2 != null)
            {
                SchemaException e = new SchemaException("Missing tables");
                e.SyncStage = SyncStage.ReadingSchema;
                e.ErrorNumber = SyncErrorNumber.MissingTableSchema;
                throw e;
            }

            return schema;
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

        protected virtual SyncSchema GetSchemaFromDatabase(Collection<string> tableNames, out Collection<string> missingTables)
        {
            SyncSchema schema = new SyncSchema();
            schema.SchemaDataSet = new DataSet();
            schema.SchemaDataSet.Locale = CultureInfo.InvariantCulture;
            missingTables = new Collection<string>();

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
                        dataTable = adapter.FillSchema(dataTable, null);
                        dataTable.TableName = tableName;    // rename of the table?
                        schema.SchemaDataSet.Tables.Add(dataTable);
                    }
                    catch (Exception)
                    {
                        missingTables.Add(tableName);
                    }
                }
                else
                {
                    missingTables.Add(tableName);
                }
            }

            if (missingTables.Count == 0)
                missingTables = null;

            return schema;
        }
    }
}
