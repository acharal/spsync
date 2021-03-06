﻿using System;
using System.Data;
using Sp.Data;
using Sp.Data.Caml;
using System.Collections.Generic;
using System.Globalization;
using System.Collections.ObjectModel;
using Microsoft.Synchronization.Data;
using System.Xml.Serialization;
using Sp.Sync.Data.Server;

namespace Sp.Sync.Data
{
    /// <summary>
    /// The SpSyncAdapter serves as the bridge between the synchronization provider
    /// and the connection to a sharepoint server.
    /// </summary>
    public class SpSyncAdapter
    {
        /// <summary>
        /// Gets a collection of ColumnMapping objects for the table. 
        /// These objects map columns in a server table to the corresponding columns in a client table.
        /// </summary>
        public SyncColumnMappingCollection ColumnMappings { get; set; }

        /// <summary>
        /// Gets a collection of TypeMapping object for tha table
        /// These objects maps the sharepoint server fields to the corresponding datatype columns in a sql table.
        /// </summary>
        public SyncTypeMappingCollection TypeMappings { get; set; }

        /// <summary>
        /// Gets or sets the name of the Sharepoint list
        /// </summary>
        public string ListName { set; get; }

        /// <summary>
        /// Gets or sets the name of the Sharepoint view
        /// </summary>
        public string ViewName { set; get; }

        /// <summary>
        /// Gets or sets the name of the sync table name
        /// </summary>
        public string TableName { set; get; }

        /// <summary>
        /// Gets or sets the description of the synchronization adapter
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// Gets or sets the option to include hidden field from the Sharepoint list
        /// </summary>
        public bool IncludeHiddenColumns { set; get; }

        /// <summary>
        /// Gets or sets the Filter Caml Query
        /// </summary>
        public string FilterClause { set; get; }

        /// <summary>
        /// Gets the names of the columns that the table contains.
        /// </summary>
        /// <remarks>Used to project a sharepoint list to certain columns.</remarks>
        public SyncDataColumnCollection DataColumns { get; set; }

        /// <summary>
        /// Gets the names of the columns to be ignored on update.
        /// </summary>
        /// <remarks></remarks>
        public SyncDataColumnCollection IgnoreColumnsOnUpdate { get; set; }

        /// <summary>
        /// Gets or sets the name of a rowguid column
        /// </summary>
        public string RowGuidColumn { get; set; }

        /// <summary>
        /// Gets or sets the option to include or exclude MetaInfo properties
        /// </summary>
        /// <remarks>Used by the sync provider to store ReplicationID</remarks>
        public bool IncludeProperties { get; set; }

        /// <summary>
        /// Gets or sets the current transaction to the sharepoint server
        /// </summary>
        [XmlIgnore]
        public SpTransaction Transaction { set; get; }

        public SpSyncAdapter()
        { 
        
        }
        
        /// <summary>
        /// Initializes a new SpSyncAdapter object and adapts it to a list
        /// </summary>
        /// <param name="listName"></param>
        public SpSyncAdapter(string listName)
            : this(listName, null)
        {
        }

        public SpSyncAdapter(string listName, string viewName)
        {
            if (listName == null)
                throw new ArgumentNullException("listName");

            ListName = listName;
            TableName = listName;
            ViewName = viewName;
            ColumnMappings = new SyncColumnMappingCollection();
            TypeMappings = new SyncTypeMappingCollection();
            DataColumns = new SyncDataColumnCollection();
        }

        /// <summary>
        /// Populates the schema information for the table that is specified in TableName.
        /// </summary>
        /// <param name="table">the table to populate</param>
        /// <param name="connection">the connection object to sharepoint</param>
        /// <returns>a datatable object that contains the schema information</returns>
        public DataTable FillSchema(DataTable table, SpConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            if (table == null)
            {
                table = new DataTable();
                table.Locale = CultureInfo.InvariantCulture;
            }

            FillSchemaFromSharepoint(table, connection);

            
            if (IncludeProperties && DataColumns.Count > 0)
            {
                foreach (string dataColumn in DataColumns)
                {
                    string fieldName = GetServerColumnFromClientColumn(dataColumn);
                    if (IsMetaInfoProperty(fieldName))
                    {
                        DataColumn column = new DataColumn(dataColumn, typeof(String));
                        SetDataColumnExtendedProperty(column, "DataTypeName", "nvarchar");
                        SetDataColumnExtendedProperty(column, "ColumnLength", "50");

                        if (column.ColumnName == this.RowGuidColumn)
                        {
                            column.DataType = typeof(Guid);
                            SetDataColumnExtendedProperty(column, "RowGuidCol", true);
                            SetDataColumnExtendedProperty(column, "DataTypeName", "uniqueidentifier");
                        }
                        table.Columns.Add(column);
                    }
                }
            }
            
            MapFromServerToClient(table);

            return table;
        }

        #region Column Mappings
        /// <summary>
        /// Gets the client column name that corresponds to the specified server column name. 
        /// </summary>
        /// <param name="serverColumn">the name of the column in server</param>
        /// <returns>the name of the column in client</returns>
        public string GetClientColumnFromServerColumn(string serverColumn)
        {
            int index = ColumnMappings.IndexOfServerColumn(serverColumn);

            if (index > -1)
                return ColumnMappings[index].ClientColumn;
            else if (this.DataColumns.Contains(serverColumn))
                return serverColumn;
            else
                return null;
        }

        /// <summary>
        /// Gets the client column name that corresponds to the specified server column name. 
        /// </summary>
        /// <param name="serverColumn">the name of the column in server</param>
        /// <returns>the name of the column in client</returns>
        public string GetServerColumnFromClientColumn(string clientColumn)
        {
            int index = ColumnMappings.IndexOfClientColumn(clientColumn);

            if (index > -1)
                return ColumnMappings[index].ServerColumn;
            else if (this.DataColumns.Contains(clientColumn) && this.ColumnMappings.IndexOfServerColumn(clientColumn) < 0)
                return clientColumn;
            else
                return null;
        }

        /// <summary>
        /// Change the column names of the datatable according to the ColumnMappings
        /// </summary>
        /// <param name="dataTable">the DataTable object to be changed</param>
        protected void MapFromServerToClient(DataTable dataTable)
        {
            dataTable.TableName = this.TableName;
            if (this.ColumnMappings != null)
            {
                foreach (DataColumn column in dataTable.Columns)
                    column.ColumnName = GetClientColumnFromServerColumn(column.ColumnName);
            }
        }

        /// <summary>
        /// Change the column names of the datatable according to the ColumnMappings
        /// </summary>
        /// <param name="dataTable">the DataTable object to be changed</param>
        protected void MapFromClientToServer(DataTable dataTable)
        {
            dataTable.TableName = this.TableName;
            if (this.ColumnMappings != null)
            {
                foreach (DataColumn column in dataTable.Columns)
                    column.ColumnName = GetServerColumnFromClientColumn(column.ColumnName);
            }
        }

        /// <summary>
        /// Sets the properties of the column according to the properties of a sharepoint field
        /// </summary>
        /// <param name="column">the DataColumn object to change</param>
        /// <param name="field">the Sharepoint Field</param>
        protected void MapFromFieldToColumn(DataColumn column, Field field)
        {
            int num = this.TypeMappings.IndexOfFieldType(field.FieldType);
            TypeMapping typeMapping;
 
            if (num > -1)
            {
                typeMapping = this.TypeMappings[num];
            }
            else if (this.TypeMappings.DefaultMapping != null)
            {
                typeMapping = this.TypeMappings.DefaultMapping;
            }
            else
            {
                throw new NotImplementedException("there is no default type mapping");
            }

            column.ColumnName = field.Name;
            column.Caption    = field.DisplayName;
            column.DataType = typeMapping.Type;

            SetDataColumnExtendedProperty(column, "DataTypeName", typeMapping.SqlType);

            if (typeMapping.SqlLength != null)
               SetDataColumnExtendedProperty(column, "ColumnLength", typeMapping.SqlLength);

            if (column.ColumnName == this.RowGuidColumn)
                SetDataColumnExtendedProperty(column, "RowGuidCol", true);
        }

        #endregion

        #region Sharepoint specific methods
        /// <summary>
        /// Fills the schema of the table consulting the schema of the sharepoint list.
        /// </summary>
        /// <param name="table">the DataTable object to change</param>
        /// <param name="connection">an open Sharepoint connection</param>
        protected void FillSchemaFromSharepoint(DataTable table, SpConnection connection)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            ViewDef viewDef = null;
            ListDef listDef = null;

            if (this.ViewName != null)
            {
                viewDef = connection.GetViewSchema(this.ListName, this.ViewName);
                listDef = viewDef.ListDef;
            }
            else
            {
                listDef = connection.GetListSchema(this.ListName);
            }

            IList<DataColumn> primaryColumns = new List<DataColumn>();

            foreach (Field field in listDef.Fields)
            {
                DataColumn column;

                if (viewDef != null && 
                    viewDef.ViewFields.Count > 0 && 
                    !ContainsFieldRefWithName(viewDef.ViewFields, field.Name))
                    continue;

                if (!table.Columns.Contains(field.Name))
                {
                    column = new DataColumn();
                    table.Columns.Add(column);
                }
                else
                {
                    column = table.Columns[field.Name];
                }

                MapFromFieldToColumn(column, field);

                // sharepoint returns as primary key the ID
                // which is relevant to sharepoint list.
                if (field.IsPrimaryKey)
                    primaryColumns.Add(column);
            }

            bool primaryRemoved = false;

            if (DataColumns.Count > 0)
            {
                List<string> columnsToRemove = new List<string>();
                foreach (DataColumn tableColumn in table.Columns)
                {
                    if (!DataColumns.Contains(tableColumn.ColumnName))
                    {
                        columnsToRemove.Add(tableColumn.ColumnName);

                        if (primaryColumns.Contains(tableColumn))
                            primaryRemoved = true;
                    }
                }

                foreach (string tableColumn2 in columnsToRemove)
                    table.Columns.Remove(tableColumn2);              
            }

            if (primaryColumns.Count > 0 && !primaryRemoved)
            {
                DataColumn[] primaryKey = new DataColumn[primaryColumns.Count];
                primaryColumns.CopyTo(primaryKey, 0);
                table.PrimaryKey = primaryKey;
            }

            table.TableName = this.TableName;
        }


        /// <summary>
        ///  Fills the tables insertTbl, updateTbl, deleteTbl with the changes fetch by the sharepoint server
        ///  since a change token 
        /// </summary>
        /// <param name="anchor">the anchor to specify the change token</param>
        /// <param name="rowLimit">the maximum number of rows to fetch </param>
        /// <param name="connection">the connection to the sharepoint server</param>
        /// <param name="insertTbl">the DataTable to append the rows that have been inserted</param>
        /// <param name="updateTbl">the DataTable to append the rows that have been updated</param>
        /// <param name="deleteTbl">the DataTable to append the rows that have been deleted</param>
        /// <remarks>
        /// Because of the response of the sharepoint changelog we cannot identify the updates from the inserts.
        /// So, no record will be added to the updateTbl.
        /// </remarks>
        /// <returns>the new SpSyncAnchor object to be used in subsequent calls</returns>
        /// #DOWNLOAD
        public SpSyncAnchor SelectIncremental(SpSyncAnchor anchor, int rowLimit, SpConnection connection,
            DataTable insertTbl, DataTable updateTbl, DataTable deleteTbl)
        {
            if (anchor == null)
                throw new ArgumentNullException("anchor");
            if (connection == null)
                throw new ArgumentNullException("connection");

            QueryOptions queryOptions = new QueryOptions()
            {
                PagingToken = anchor.PagingToken,
                DateInUtc = false
            };

            IEnumerable<string> viewFields = GetViewFields();

            ChangeBatch changes = connection.GetListItemChangesSinceToken(
                this.ListName,
                this.ViewName,
                FilterClause,
                viewFields,
                IncludeProperties,
                rowLimit,
                queryOptions,
                anchor.NextChangesToken);

            if (insertTbl != null)
            {
                foreach (ListItem item in changes.ChangedItems)
                {
                    DataRow row = insertTbl.NewRow();
                    Exception e;
                    MapListItemToDataRow(item, row, out e);
                    if (e != null)
                    {
                        if (SyncTracer.IsErrorEnabled())
                            SyncTracer.Error(e.ToString());
                    }
                    insertTbl.Rows.Add(row);
                }
            }

            // FIX: Cannot identify the updates from the inserts.

            if (deleteTbl != null)
            {
                foreach (ChangeItem item in changes.ChangeLog)
                {
                    if (ChangeCommands.IsDelete(item.Command))
                    {
                        DataRow row = deleteTbl.NewRow();
                        // FIX: Probably the ID is not mapped at all to the client table
                        row[deleteTbl.PrimaryKey[0]] = item.ListItemID;
                        deleteTbl.Rows.Add(row);
                    }
                }
            }
            insertTbl.AcceptChanges(); // COMMITCHANGES
            updateTbl.AcceptChanges();
            deleteTbl.AcceptChanges();
            return CalculateNextAnchor(anchor, changes);
        }

        /// #DOWNLOAD
        public SpSyncAnchor SelectIncremental(SpSyncAnchor anchor, int rowLimit, SpConnection connection,
            DataTable changeTable)
        {//#DOWNLOAD in batches - step 3

            if (anchor == null)
                throw new ArgumentNullException("anchor");
            if (connection == null)
                throw new ArgumentNullException("connection");

            QueryOptions queryOptions = new QueryOptions()
            {
                PagingToken = anchor.PagingToken,
                DateInUtc = false
            };

            IEnumerable<string> viewFields = GetViewFields();

            ChangeBatch changes = connection.GetListItemChangesSinceToken(
                this.ListName,
                this.ViewName,
                FilterClause,
                viewFields,
                IncludeProperties,
                rowLimit,
                queryOptions,
                anchor.NextChangesToken);

            foreach (ListItem item in changes.ChangedItems)
            {
                DataRow row = changeTable.NewRow();
                Exception e;
                MapListItemToDataRow(item, row, out e);
                if (e != null)
                {
                    if (SyncTracer.IsErrorEnabled())
                        SyncTracer.Error(e.ToString());
                }
                changeTable.Rows.Add(row);
                row.AcceptChanges();
                row.SetModified();
            }

            foreach (ChangeItem item in changes.ChangeLog)
            {
                string clientColumnName = GetClientColumnFromServerColumn("ID");
                if (ChangeCommands.IsDelete(item.Command))
                {
                    DataRow row = changeTable.NewRow();
                    // FIX: Probably the ID is not mapped at all to the client table
                    row[clientColumnName] = item.ListItemID;
                    changeTable.Rows.Add(row);
                    row.AcceptChanges();
                    row.Delete();
                }
            }
            
            return CalculateNextAnchor(anchor, changes);
        }

        /// #DOWNLOAD (not in batches)
        public SpSyncAnchor SelectAll(SpSyncAnchor anchor, int rowLimit, DataTable dataTable, SpConnection connection)
        {

            if (anchor == null)
                throw new ArgumentNullException("anchor");

            if (connection == null)
                throw new ArgumentNullException("connection");

            if (dataTable == null)
                throw new ArgumentNullException("dataTable");

            QueryOptions queryOptions = new QueryOptions()
            {
                PagingToken = anchor.PagingToken,
                DateInUtc = false
            };

            IEnumerable<string> viewFields = GetViewFields();

            ListItemCollection listItems = connection.GetListItems(
                this.ListName,
                this.ViewName,
                this.FilterClause,
                viewFields,
                IncludeProperties,
                rowLimit,
                queryOptions);

            if (dataTable != null)
            {
                foreach (ListItem item in listItems)
                {
                    DataRow row = dataTable.NewRow();
                    Exception e;
                    MapListItemToDataRow(item, row, out e);
                    if (e != null)
                    {
                        if (SyncTracer.IsErrorEnabled())
                            SyncTracer.Error(e.ToString());
                    }
                    dataTable.Rows.Add(row);
                }
            }
            dataTable.AcceptChanges();
            return CalculateNextAnchor(anchor, listItems.NextPage);
        }
        private void CopyRows(DataTable origin, DataTable destination, int firstRowIndex,int RowsCount)
        {
            try
            {
                if (origin.Rows.Count < (firstRowIndex + RowsCount))
                    RowsCount = origin.Rows.Count - firstRowIndex;
                for (int i = firstRowIndex; i < firstRowIndex + RowsCount; i++)
                {
                    destination.ImportRow(origin.Rows[i]);
                }
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inserts"></param>
        /// <param name="updates"></param>
        /// <param name="deletes"></param>
        /// <param name="connection"></param>
        /// #UPLOAD 3
        public void Update(DataTable changes, SpConnection connection, out Collection<SyncConflict> errors)
        {

            errors = new Collection<SyncConflict>();

            if (changes == null)
                throw new ArgumentNullException("changes");

            if (connection == null)
                throw new ArgumentNullException("connection");

            int _batchSize = 25;
            int segmentsCount = (int)Math.Round( Math.Ceiling((double)changes.Rows.Count/ _batchSize),0);


            if (IgnoreColumnsOnUpdate != null)
            {
                // case to be handled
                // cannot remove Sharepoint ID.
                // cannot remove Primary Key of DataTable?

                foreach (string ignoredColumn in IgnoreColumnsOnUpdate)
                {
                    string clientColumn = GetClientColumnFromServerColumn(ignoredColumn);
                    if (clientColumn != null &&
                        changes.Columns.Contains(clientColumn))
                    {
                        changes.Columns.Remove(clientColumn);
                    }
                }
            }


            DataTable changesTotal = changes.Copy();
            
            for (int i = 0; i < segmentsCount; i++)
            {
                changes.Rows.Clear();

                CopyRows(changesTotal, changes, i * _batchSize, _batchSize);

                //SEND SEGMENT 
                UpdateBatch batch = new UpdateBatch();

                string clientIdColumn = GetClientColumnFromServerColumn("ID");

                if (!changes.Columns.Contains(clientIdColumn))
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, Messages.ColumnIDNotContained, clientIdColumn));

                IDictionary<int, DataRow> IdMapping = new Dictionary<int, DataRow>();

                foreach (DataRow row in changes.Rows)
                {
                    UpdateItem u = batch.CreateNewItem();

                    switch (row.RowState)
                    {
                        case DataRowState.Added:
                            u.Command = UpdateCommands.Insert;
                            break;
                        case DataRowState.Deleted:
                            u.Command = UpdateCommands.Delete;
                            break;
                        case DataRowState.Modified:
                            u.Command = UpdateCommands.Update;
                            break;
                        case DataRowState.Unchanged:
                            continue;
                    }

                    if (u.Command == UpdateCommands.Delete)
                        row.RejectChanges();

                    if (u.Command != UpdateCommands.Insert)
                    {
                        if (!(row[clientIdColumn] is DBNull))
                        {
                            u.ListItemID = (int)row[clientIdColumn];
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (u.Command != UpdateCommands.Delete)
                    {
                        ListItem item = new ListItem();
                        Exception e;

                        MapDataRowToListItem(row, item, out e);
                        u.ChangedItemData = item;
                        if (e != null && SyncTracer.IsErrorEnabled())
                            SyncTracer.Error(e.ToString());
                    }

                    batch.Add(u);
                    IdMapping[u.ID] = row;

                    if (u.Command == UpdateCommands.Delete)
                        row.Delete();
                }

                if (batch.Count != 0)
                {
                    //try
                    //{
                        UpdateResults results = connection.UpdateListItems(this.ListName, batch);
                        // FIX: errors must be handled appropriately
                        foreach (UpdateResult r in results)
                        {
                            if (!r.IsSuccess())
                            {
                                if (!IdMapping.ContainsKey(r.UpdateItemID))
                                    throw new InvalidOperationException(
                                        String.Format(CultureInfo.CurrentCulture, Messages.NoIDMapping, r.UpdateItemID));

                                DataRow clientRow = IdMapping[r.UpdateItemID];
                                errors.Add(CreateSyncError(r, clientRow));
                            }
                        }
                    //}
                    //catch (Exception ex)
                    //{ 
                    ////usually connection error
                    //    foreach (UpdateItem item in batch)
                    //    {
                    //        if (!IdMapping.ContainsKey(item.ID))
                    //            throw new InvalidOperationException(
                    //                String.Format(CultureInfo.CurrentCulture, Messages.NoIDMapping, r.UpdateItemID));

                    //        DataRow clientRow = IdMapping[item.ID];
                    //        errors.Add(CreateSyncError(new UpdateResult(, clientRow));
                    //    }
                    //}
                }
                //END SEND SEGMENT 
            }
            
            if (errors.Count == 0)
                errors = null;

        }

        private SyncConflict CreateSyncError(UpdateResult r, DataRow clientRow)
        {
            Microsoft.Synchronization.SyncStage syncStage = Microsoft.Synchronization.SyncStage.UploadingChanges;

            switch (r.Command)
            {
                case "New":
                    syncStage = Microsoft.Synchronization.SyncStage.ApplyingInserts;
                    break;
                case "Update":
                    syncStage = Microsoft.Synchronization.SyncStage.ApplyingUpdates;
                    break;
                case "Delete":
                    syncStage = Microsoft.Synchronization.SyncStage.ApplyingDeletes;
                    break;
            }

            SyncConflict conflict;

            if (r.ErrorCode == UpdateResult.VersionConflict)
            {
                if (r.Command == "Update")
                    conflict = new SyncConflict(ConflictType.ClientUpdateServerUpdate, syncStage);
                else if (r.Command == "Delete")
                    conflict = new SyncConflict(ConflictType.ClientDeleteServerUpdate, syncStage);
                else
                    conflict = new SyncConflict(ConflictType.Unknown, syncStage);

                if (r.ItemData != null)
                { 
                    Exception e;
                    DataRow serverRow = clientRow.Table.NewRow();
                    MapListItemToDataRow(r.ItemData, serverRow, out e);
                    if (e == null)
                    {
                        conflict.ServerChange = serverRow.Table.Clone();
                        conflict.ServerChange.TableName = this.TableName;
                        conflict.ServerChange.Rows.Add(serverRow);
                    }
                }
            }
            else if (r.ErrorCode == UpdateResult.ItemDeleted)
            {
                conflict = new SyncConflict(ConflictType.ClientUpdateServerDelete, syncStage);
            }
            else
            {
                conflict = new SyncConflict(ConflictType.ErrorsOccurred, syncStage);
            }

            if (conflict.ClientChange == null)
            {
                conflict.ClientChange = clientRow.Table.Clone();
                conflict.ClientChange.TableName = clientRow.Table.TableName;
            }

            conflict.ClientChange.ImportRow(clientRow);
            conflict.ErrorMessage = r.ErrorMessage;

            return conflict;
        }

        /// <summary>
        /// Calculates the next SpSyncAnchor from the current SpSyncAnchor object and the change batch just returned from the server
        /// </summary>
        /// <param name="currentAnchor">the current SpSyncAnchor object</param>
        /// <param name="changes"> the ChangeBatch object</param>
        /// <returns>the new SpSyncAnchor for the next incremental select</returns>
        protected SpSyncAnchor CalculateNextAnchor(SpSyncAnchor currentAnchor, ChangeBatch changes)
        {
            SpSyncAnchor nextChanges = currentAnchor.NextChangesAnchor ?? new SpSyncAnchor(changes.NextChangeBatch, null);
            SpSyncAnchor nextAnchor = nextChanges;

            if (changes.HasMoreData())
            {
                nextAnchor = new SpSyncAnchor(currentAnchor.NextChangesToken, changes.NextPage);
                nextAnchor.NextChangesAnchor = nextChanges;
            }
            nextAnchor.HasMoreData = changes.HasMoreChanges();
            return nextAnchor;
        }

        /// <summary>
        /// Calculates the next SpSyncAnchor from the current SpSyncAnchor object and the change batch just returned from the server
        /// </summary>
        /// <param name="currentAnchor">the current SpSyncAnchor object</param>
        /// <param name="nextPageToken">the next page token given from sharepoint</param>
        /// <returns>the new SpSyncAnchor for the next incremental select</returns>
        protected SpSyncAnchor CalculateNextAnchor(SpSyncAnchor currentAnchor, string nextPageToken)
        {
            SpSyncAnchor nextChanges = SpSyncAnchor.Empty;
            SpSyncAnchor nextAnchor = nextChanges;

            if (nextPageToken != null)
            {
                nextAnchor = new SpSyncAnchor(currentAnchor.NextChangesToken, nextPageToken);
                nextAnchor.NextChangesAnchor = nextChanges;
            }

            return nextAnchor;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Parse the string to an integer value
        /// </summary>
        private int ParseIntOrLookupID(string value)
        {
            int index = value.IndexOf(";#");
            if (index > -1)
            {
                value = value.Substring(0, index);
            }
            CultureInfo culture = new CultureInfo("en-US");
            culture.NumberFormat.CurrencyDecimalSeparator = ".";
            culture.NumberFormat.CurrencyGroupSeparator = ",";
            culture.NumberFormat.NumberDecimalSeparator = ".";
            culture.NumberFormat.NumberGroupSeparator = ",";
            culture.NumberFormat.PercentDecimalSeparator = ".";
            culture.NumberFormat.PercentGroupSeparator = ",";
            value = value.Replace(',','.');
            return Int32.Parse(value, NumberStyles.AllowDecimalPoint, culture);
        }
        
        /// <summary>
        /// Parse the string to a float value
        /// </summary>
        private float ParseFloat(string value)
        {
            int index = value.IndexOf(";#");
            if (index > -1)
            {
                value = value.Substring(0, index);
            }
            CultureInfo culture = new CultureInfo("en-US");
            culture.NumberFormat.CurrencyDecimalSeparator = ".";
            culture.NumberFormat.CurrencyGroupSeparator = ",";
            culture.NumberFormat.NumberDecimalSeparator = ".";
            culture.NumberFormat.NumberGroupSeparator = ",";
            culture.NumberFormat.PercentDecimalSeparator = ".";
            culture.NumberFormat.PercentGroupSeparator = ",";
            value = value.Replace(',','.');
            return float.Parse(value, NumberStyles.AllowDecimalPoint, culture);
        }

        
        /// <summary>
        /// Sets the extended properties of a DataColumn object
        /// </summary>
        /// <param name="column">the DataColumn object</param>
        /// <param name="property">the name of the property</param>
        /// <param name="value">the value of the property</param>
        private void SetDataColumnExtendedProperty(DataColumn column, string property, object value)
        {
            if (column.ExtendedProperties[property] == null)
                column.ExtendedProperties.Add(property, value);
            else
                column.ExtendedProperties[property] = value;
        }

        /// <summary>
        /// Try to fill a DataRow object from a list item
        /// </summary>
        /// <param name="item">the ListItem object</param>
        /// <param name="row">the DataRow object</param>
        /// <param name="e">the first exception of conversion or null for success conversion</param>
        private void MapListItemToDataRow(ListItem item, DataRow row, out Exception e)
        {
            e = null;

            if (item == null)
                throw new ArgumentNullException("item");

            if (row == null)
                throw new ArgumentNullException("row");

            foreach (KeyValuePair<string, string> cell in item.Fields)
            {
                DataColumn col = null;
                string columnName = GetClientColumnFromServerColumn(cell.Key);

                if (columnName == null)
                    continue;

                if (row.Table.Columns.Contains(columnName))
                    col = row.Table.Columns[columnName];

                try
                {
                    if (col != null)
                    {
                       
                        if (col.DataType == typeof(String))
                            row[col] = cell.Value;
                        else if (col.DataType == typeof(int))
                            row[col] = ParseIntOrLookupID(cell.Value); // lookup
                        else if (col.DataType == typeof(Guid))
                            row[col] = new Guid(cell.Value);
                        else if (col.DataType == typeof(DateTime))
                            row[col] = DateTime.Parse(cell.Value);
                        else if (col.DataType == typeof(Boolean))
                            row[col] = (cell.Value == "1" ? true : false);
                        else if (col.DataType == typeof(float) || col.DataType == typeof(double))
                            row[col] = ParseFloat(cell.Value);
                        else
                            row[col] = cell.Value;
                    }
                }
                catch (Exception ex)
                {
                    if (e == null)
                        e = ex;
                    e = new Exception(ex.Message + " || LLLLLLL " + row.Table.TableName + row.Table.Columns[columnName].DataType.ToString() + "," + col.ColumnName + "," + cell.Value.ToString() + "," + col.DataType.ToString() + "," + cell.Key + "," + cell.GetType().ToString() + ",|" + CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator + ",|" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                }
            }
        }

        /// <summary>
        /// Try to fill a ListItem object from a datarow
        /// </summary>
        /// <param name="item">the ListItem object</param>
        /// <param name="row">the DataRow object</param>
        /// <param name="e">the first exception of conversion or null for success conversion</param>
        private void MapDataRowToListItem(DataRow row, ListItem item, out Exception e)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (row == null)
                throw new ArgumentNullException("row");

            e = null;

            if (item.Fields == null)
            {
                item.Fields = new List<KeyValuePair<string, string>>();
            }

            foreach (DataColumn column in row.Table.Columns)
            {
                string fieldName = GetServerColumnFromClientColumn(column.ColumnName);

                if (fieldName == null)
                    continue;
                
                if (this.DataColumns.Count > 0 &&
                    !this.DataColumns.Contains(fieldName))
                    continue;

                string fieldValue;
                if (column.DataType == typeof(DateTime) && row[column] != null)
                    //fieldValue = ((DateTime)row[column]).ToString("yyyy-MM-ddTHH:mm:ssZ");
                //FIX FOR DBNULL DATETIME
                {
                    if (row[column] is DBNull)
                        fieldValue = (DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ssZ");
                    else
                        fieldValue = ((DateTime)row[column]).ToString("yyyy-MM-ddTHH:mm:ssZ");
                }
                //END OF FIX FOR DBNULL DATETIME
                else
                    fieldValue = row[column].ToString();

                item.Fields.Add(new KeyValuePair<string, string>(fieldName, fieldValue));
                // FIX: Is there any columns that must not mapped to fields? 
                // this maybe result to error from the sharepoint-side
            }
        }

        /*
        /// <summary>
        /// Adds to update batch the update items for each row in the table
        /// </summary>
        /// <param name="batch">the batch to append update items</param>
        /// <param name="table">the table to enumerate</param>
        /// <param name="command">the command to tag each update item</param>
        private void AddUpdateItems(UpdateBatch batch, DataTable table, string command)
        {
            if (batch == null)
                throw new ArgumentNullException("batch");
            if (table == null)
                throw new ArgumentNullException("table");

            if (command != UpdateCommands.Insert
                || command != UpdateCommands.Delete
                || command != UpdateCommands.Update)
                throw new ArgumentOutOfRangeException("command", "Invalid command");


            string clientIdColumn = GetClientColumnFromServerColumn("ID");

            if (command != UpdateCommands.Insert)
            {
                if (!table.Columns.Contains(clientIdColumn))
                    throw new InvalidOperationException("Column ID " + clientIdColumn + " is not contained to table");
            }

            foreach (DataRow row in table.Rows)
            {
                UpdateItem u = batch.CreateNewItem();
                u.Command = command;
                if (u.Command != UpdateCommands.Insert)
                {
                    u.ListItemID = (int)row[clientIdColumn];
                }

                if (u.Command != UpdateCommands.Delete)
                {
                    ListItem item = new ListItem();
                    Exception e;
                    MapDataRowToListItem(row, item, out e);

                    if (e != null && SyncTracer.IsErrorEnabled())
                        SyncTracer.Error(e.ToString());
                }

                batch.Add(u);
            }
        }
        */

        private bool ContainsFieldRefWithName(List<FieldRef> fieldRefs, string Name)
        {
            foreach (FieldRef f in fieldRefs)
                if (f.Name == Name)
                    return true;

            return false;
        }

        private bool IsMetaInfoProperty(string fieldName)
        {
            return fieldName.StartsWith("MetaInfo_");
        }

        private IEnumerable<string> GetViewFields()
        {
            List<string> viewFields = new List<string>();
            bool includeMetaInfo = IncludeProperties;

            foreach (string columnName in this.DataColumns)
            {
                if (columnName != null)
                {
                    if (IsMetaInfoProperty(columnName))
                    {
                        includeMetaInfo = true;
                    }
                    viewFields.Add(columnName);
                }
            }

            if (includeMetaInfo)
            {
                viewFields.Add("MetaInfo");
            }

            return viewFields;
        }
        
        #endregion
    }
}
