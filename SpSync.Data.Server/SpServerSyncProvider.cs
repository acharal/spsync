using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization.Data;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using SpCaml.DataAccess.Caml;
using SpCaml.DataAccess.Interface;
using SpCaml.DataAccess.Implementation.WS;

namespace SpSync.Data.Server
{
    /* Issues To be handled:
     * - ReadOnly and computed fields are not updated
     * - New and updated records cannot be distiguished
     * - Versioning of list schema (aka schema changes) is not handled appropriately
     * - No easy way to compute the total number of the batches of changes.
     * - Problem with the ID of the record when uploading new records to sharepoint. IDs are assigned by the server.
     * - No way to detect only the changed fields. Sharepoint returns the whole record if it updated one field.
     * - In some cases (if syncanchor is not valid or too old) we must somehow invoke a FULL resync.
     * - No easy way to detect that the downloaded changes might be occured from a previous update of the same client.
     */
    public class SpServerSyncProvider : ServerSyncProvider
    {
        private SyncSchema _schema = null;

        protected SyncSchema Schema
        {
            get
            {
                if (_schema != null)
                    return _schema;
                else
                {
                    _schema = 
                    GetSchema(
                               new Collection<string>(GetServerInfo(null).TablesInfo.Select(t => t.TableName).ToArray()),
                               null);
                    return _schema;
                }
            }

            set { _schema = value; }
            
        }

        public System.Net.ICredentials ServiceCredentials = System.Net.CredentialCache.DefaultCredentials;

        public int BatchSize = 100;

        private ISpListAdapter _Adapter = null;

        public ISpListAdapter SpAdapter
        {
            set {
                _Adapter = value;
                _Adapter.BatchCount = BatchSize;
            }

            get {
                return _Adapter;
            }
        }

        public override SyncContext ApplyChanges(SyncGroupMetadata groupMetadata, DataSet dataSet, SyncSession syncSession)
        {
            SyncContext syncContext = new SyncContext();
            // we have to update statistics appropriatelly
            // SyncContext must return the dataset of the changes (?)
            
            foreach (SyncTableMetadata tableMetadata in groupMetadata.TablesMetadata)
            {
                //ListServices.UpdateListItems(tableMetadata.TableName, null);
                ApplyChanges(tableMetadata, dataSet, syncSession);
            }
            return syncContext;
        }

        private void ApplyChanges(SyncTableMetadata tableMetadata, DataSet dataSet, SyncSession syncSession)
        {
            Lists ListServices = new Lists();
            ListServices.Credentials = ServiceCredentials;
            DataTable dataTable = dataSet.Tables[tableMetadata.TableName];

            DataTable deletes = dataTable.GetChanges(DataRowState.Deleted);

            //UpdateBatch updateBatch = new UpdateBatch();
            List<UpdateItem> updateItems = new List<UpdateItem>();
            int i = 0;
            if (deletes != null)
            {
                foreach (DataRow deletedRow in deletes.Rows)
                {
                    i++;
                    UpdateItem item = new UpdateItem();
                    item.ID = i;
                    item.Command = "Delete";
                    item.ListItemID = (int)deletedRow["ID", DataRowVersion.Original];
                    updateItems.Add(item);
                }
            }

            DataTable updates = dataTable.GetChanges(DataRowState.Modified);
            if (updates != null)
            {
                foreach (DataRow updatedRow in updates.Rows)
                {
                    i++;
                    UpdateItem item = new UpdateItem();
                    item.ID = i;
                    item.Command = "Update";
                    item.ListItemID = (int)updatedRow["ID", DataRowVersion.Original];
                    item.ChangedItemData = new ListItem();
                    item.ChangedItemData.ID = item.ListItemID;
                    item.ChangedItemData.Fields = new List<KeyValuePair<string, string>>();

                    foreach (DataColumn col in updates.Columns)
                    {
                        item.ChangedItemData.Fields.Add(new KeyValuePair<string, string>(col.ColumnName, updatedRow[col].ToString()));
                    }
                    updateItems.Add(item);
                }
            }

            DataTable inserts = dataTable.GetChanges(DataRowState.Added);

            if (inserts != null)
            {
                foreach (DataRow addedRow in inserts.Rows)
                {
                    i++;
                    UpdateItem item = new UpdateItem();
                    item.ID = i;
                    item.Command = "New";
                    //item.ListItemID = (int)addedRow["ID", DataRowVersion.Original];
                    item.ChangedItemData = new ListItem();
                    //item.ChangedItemData.ID = item.ListItemID;
                    item.ChangedItemData.Fields = new List<KeyValuePair<string, string>>();

                    foreach (DataColumn col in inserts.Columns)
                    {
                        object o = addedRow[col];
                        string v = o is System.Guid ? ((Guid)o).ToString("B").ToUpper() : o.ToString();

                        if (o is System.DBNull)
                            continue;

                        item.ChangedItemData.Fields.Add(new KeyValuePair<string, string>(col.ColumnName, v));
                    }
                    updateItems.Add(item);
                }
            }
            UpdateBatch updateBatch = new UpdateBatch(updateItems);
            updateBatch.ContinueOnError = true;
            XElement result = ListServices.UpdateListItems(tableMetadata.TableName, updateBatch.GetCamlUpdateBatch()).GetXElement();
            
            ListServices.Dispose();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override SyncContext GetChanges(SyncGroupMetadata groupMetadata, SyncSession syncSession)
        {
            SyncContext context = new SyncContext();
            DataSet dataSet = new DataSet();

            context.DataSet = dataSet;
            context.GroupProgress = new SyncGroupProgress(groupMetadata, dataSet);

            foreach (SyncTableMetadata tableMetadata in groupMetadata.TablesMetadata)
            {
                DataTable table = GetChanges(tableMetadata, syncSession, context);
                dataSet.Tables.Add(table);
            }

            return context;
        }

        private DataTable GetChanges(SyncTableMetadata tableMetadata, SyncSession syncSession, SyncContext syncContext)
        {
            // the syncSessions.SyncParameters can be used to filter fields using the viewFields (to filter columns)
            // and the contains to filter rows. Of course a caml query can be constructed

            DataTable inserts = Schema.SchemaDataSet.Tables[tableMetadata.TableName].Clone();
            DataTable deletes = Schema.SchemaDataSet.Tables[tableMetadata.TableName].Clone();

            SpSyncAnchor anchor = SpSyncAnchor.GetAnchor(tableMetadata.LastReceivedAnchor.Anchor);

            QueryOptions options = new QueryOptions() { PagingToken = anchor.PagingToken };
            ChangeBatch changes = new ChangeBatch();

            using (Lists listsServices = new Lists())
            {

                listsServices.Credentials = this.ServiceCredentials;

                XElement result =
                    listsServices.GetListItemChangesSinceToken(
                        tableMetadata.TableName,
                        null,
                        null,
                        new XmlDocument().CreateElement("ViewFields"),
                        BatchSize.ToString(),
                        options.GetCamlQueryOptions(),
                        anchor.NextChangesToken,
                        null).GetXElement();

                changes = result.GetCamlChangeBatch();
                
            }

            changes.CurrentChangeBatch = anchor.NextChangesToken;

            if (changes.HasSchemaChanges())
            {
                // Schema has  changed
                // need full resynchronization
            }

            foreach (ChangeItem changeItem in changes.ChangeLog)
            {
                switch (changeItem.Command)
                {
                    case "Delete":
                    case "MoveAway":
                        int ID = changeItem.ListItemID;
                        DataRow row = deletes.NewRow();
                        row[deletes.PrimaryKey[0]] = ID;
                        deletes.Rows.Add(row);
                        break;
                    case "InvalidToken":
                    case "Restore":
                        // full resynchronization
                        SyncTracer.Warning("Unknown ChangeType " + changeItem.Command);
                        break;
                    case "SystemUpdate":
                    case "Rename":
                    case "":
                        break;
                }
            }
            deletes.AcceptChanges();

            SpSyncAnchor maxAnchor = 
                anchor.NextChangesAnchor != null ? 
                    anchor.NextChangesAnchor 
                    : new SpSyncAnchor(changes.NextChangeBatch, ChangeBatch.FirstPage);

            SpSyncAnchor newAnchor = 
                changes.HasMoreData() ?
                    new SpSyncAnchor(changes.CurrentChangeBatch, changes.NextPage, anchor.PageNumber + 1)
                    : maxAnchor;

            newAnchor.NextChangesAnchor = changes.HasMoreData() ? maxAnchor : null;

            syncContext.NewAnchor =
                new SyncAnchor(SpSyncAnchor.GetBytes(newAnchor));

            syncContext.MaxAnchor =
                new SyncAnchor(SpSyncAnchor.GetBytes(maxAnchor));

            syncContext.BatchCount = changes.HasMoreData() ? anchor.PageNumber + 1 : 0;

            if (changes.ChangedItems.ItemCount != 0)
            {
                foreach (ListItem item in changes.ChangedItems)
                {
                    //DataRow r = inserts.NewRow();
                    DataRow r = ItemToDataRow(item, inserts.NewRow());
                    inserts.Rows.Add(r);
                }
                inserts.AcceptChanges();
            }

            SyncTableProgress tableProgress = syncContext.GroupProgress.FindTableProgress(tableMetadata.TableName);

            foreach (DataRow addedRow in inserts.Rows)
            {
                addedRow.SetModified();
                tableProgress.Updates++;
            }

            foreach (DataRow deletedRow in deletes.Rows)
            {
                deletedRow.Delete();
                tableProgress.Deletes++;
            }

            
            inserts.Merge(deletes);
            

            tableProgress.DataTable = inserts;

            return inserts;
        }

        public override SyncSchema GetSchema(Collection<string> tableNames, SyncSession syncSession)
        {
            DataSet dataSet = GetSchemaDataSet(tableNames, syncSession);
            Schema = new SyncSchema(dataSet);
            return Schema;
        }

        public override SyncServerInfo GetServerInfo(SyncSession syncSession)
        {
            Lists listsService = new Lists();
            listsService.Credentials = ServiceCredentials;

            XElement result = listsService.GetListCollection().GetXElement();
            ListCollection lists = result.GetCamlListCollection();
            listsService.Dispose();

            IList<SyncTableInfo> syncTableInfos =
                lists.Select(l => new SyncTableInfo(l.Name, l.Description)).ToList();

            Collection<SyncTableInfo> syncCollection = new Collection<SyncTableInfo>(syncTableInfos);
            SyncServerInfo serverInfo = new SyncServerInfo(syncCollection);
            return serverInfo;
        }

        private DataSet GetSchemaDataSet(Collection<string> tableNames, SyncSession syncSession)
        {
            Lists listsService = new Lists();
            listsService.Credentials = ServiceCredentials;
            DataSet schemaDataSet = new DataSet();

            foreach (string table in tableNames)
            {
                XmlNode n = listsService.GetList(table);
                DataTable dataTable = GetListSchemaDataTable(
                    n.GetXElement().GetCamlListDef(), 
                    syncSession);
                schemaDataSet.Tables.Add(dataTable);
            }
            
            listsService.Dispose();

            return schemaDataSet;
        }

        private DataTable GetListSchemaDataTable(ListDef listDef, SyncSession syncSession)
        {
            List<DataColumn> columns = listDef.Fields.Select(f => FieldToDataColumn(f)).ToList();

            IEnumerable<string> primaryKeys = new List<string>(new string[] { "GUID" });

            DataTable dataTable = new DataTable(listDef.List.Name);

            dataTable.Columns.AddRange(columns.ToArray<DataColumn>());

            List<DataColumn> primaryColumns = new List<DataColumn>();
            foreach (DataColumn c in dataTable.Columns)
            {
                if (primaryKeys.Contains(c.ColumnName))
                    primaryColumns.Add(c);
            }
            dataTable.PrimaryKey = primaryColumns.ToArray();
            
            //dataTable.PrimaryKey = .Where(c => primaryKeys.Contains(c.Caption)).ToArray();
            return dataTable;
        }

        private Type GetDataType(string spType)
        { 
            switch(spType)
            {
                case "Number":
                case "Integer":
                case "Counter":
                    return typeof(int);
                case "Currency":
                    return typeof(float);
                case "Boolean":
                case "Recurrence":
                    return typeof(bool);
                case "DateTime":
                    return typeof(DateTime);
                case "Text":
                case "Note":
                case "URL":
                    return typeof(string);
                case "Guid":
                    return typeof(System.Guid);
                default:
                    return typeof(string);
            }
        }

        #region DataSet helpers

        private DataColumn FieldToDataColumn(Field f)
        {
            DataColumn c = new DataColumn(f.Name)
            {
                AllowDBNull = true, //!f.IsRequired,
                Caption = f.DisplayName,
                ReadOnly = f.IsReadOnly,
                DataType = GetDataType(f.FieldType),
            };

            if (c.DataType == typeof(string))
                c.ExtendedProperties["ColumnLength"] = 255;

            c.ExtendedProperties["FieldType"] = f.FieldType;

            if (f.Name == "GUID")
                c.ExtendedProperties["RowGuidCol"] = true;

            return c;
        }

        private DataRow ItemToDataRow(ListItem item, DataRow r)
        {
            foreach (var kvp in item.Fields)
                try
                {
                    Type columnType =  r.Table.Columns[kvp.Key].DataType;
                    if (columnType == typeof(bool))
                        r[kvp.Key] = kvp.Value == "1" ? true : false;
                    else if (columnType == typeof(int))
                    {
                        //string[] s = kvp.Value.Split(".".ToCharArray(),2);
                        // changes for favor of CF
                        string[] s = kvp.Value.Split(".".ToCharArray());
                        
                        r[kvp.Key] = Int32.Parse(s.Length == 0 ? kvp.Value : s[0]);
                    }
                    else
                        r[kvp.Key] = kvp.Value;
                }
                catch (Exception e)
                {
                    //SyncTracer.Warning(e.Message);
                    //System.Diagnostics
                    //    .Trace.TraceWarning(kvp.Key + " " + e.Message) ;
                }
            return r;
        }

        #endregion

    }
}
