using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization.Data;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace SpSync.Data.Server
{
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
                    return GetSchema( 
                                new Collection<string>(GetServerInfo(null).TablesInfo.Select( t => t.TableName).ToArray()),
                                null);
            }

            set { _schema = value; }
            
        }

        private System.Net.ICredentials ServiceCredentials = System.Net.CredentialCache.DefaultCredentials;

        public override SyncContext ApplyChanges(SyncGroupMetadata groupMetadata, DataSet dataSet, SyncSession syncSession)
        {
            SpWS.Lists ListServices = new SpSync.Data.Server.SpWS.Lists();
            ListServices.Credentials = ServiceCredentials;

            foreach (SyncTableMetadata tableMetadata in groupMetadata.TablesMetadata)
            {
                //ListServices.UpdateListItems(tableMetadata.TableName, null);
            }
            ListServices.Dispose();

            return new SyncContext();
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
                DataTable table = GetListItemFromAnchor(tableMetadata, syncSession, context);
                dataSet.Tables.Add(table);
            }

            return context;
        }

        public override SyncSchema GetSchema(Collection<string> tableNames, SyncSession syncSession)
        {
            DataSet dataSet = GetSchemaDataSet(tableNames, syncSession);
            Schema = new SyncSchema(dataSet);
            return Schema;
        }

        public override SyncServerInfo GetServerInfo(SyncSession syncSession)
        {
            SpWS.Lists listsService = new SpSync.Data.Server.SpWS.Lists();
            listsService.Credentials = ServiceCredentials;

            XElement result = listsService.GetListCollection().GetXElement();
            listsService.Dispose();
            XNamespace wssNamespace = result.Name.Namespace;

            List<SyncTableInfo> syncTableInfos =
                (from list in result.Descendants(wssNamespace + "List")
                 select new SyncTableInfo(list.Attribute("Title").Value, 
                                          list.Attribute("Description").Value)
                 ).ToList();

            Collection<SyncTableInfo> syncCollection = new Collection<SyncTableInfo>(syncTableInfos);
            SyncServerInfo serverInfo = new SyncServerInfo(syncCollection);

            return serverInfo;
        }

        private DataSet GetSchemaDataSet(Collection<string> tableNames, SyncSession syncSession)
        {
            SpWS.Lists listsService = new SpSync.Data.Server.SpWS.Lists();
            listsService.Credentials = ServiceCredentials;
            DataSet schemaDataSet = new DataSet();

            foreach (string table in tableNames)
            {
                XmlNode node = listsService.GetList(table);
                DataTable dataTable = GetListSchemaDataTable(node, syncSession);
                schemaDataSet.Tables.Add(dataTable);
            }
            
            listsService.Dispose();

            return schemaDataSet;
        }

        private DataTable GetListSchemaDataTable(XmlNode listDef, SyncSession syncSession)
        {
            XElement node = listDef.GetXElement();
            XNamespace xmlns = node.Name.Namespace;

            var fields = node.Element(xmlns + "Fields").Elements(xmlns + "Field");
            List<DataColumn> columns =
                (from field in fields
                 select new DataColumn(field.Attribute("Name").Value) {
                    AllowDBNull = field.Attribute("Required") != null ?  field.Attribute("Required").Value.ToUpper() != "YES" : true,
                    Caption     = field.Attribute("DisplayName") != null ?  field.Attribute("DisplayName").Value : field.Attribute("Name").Value,
                    ReadOnly    = field.Attribute("ReadOnly") != null ? field.Attribute("ReadOnly").Value.ToUpper() == "YES" : false,
                    DataType    = field.Attribute("Type") != null ? GetDataType(field.Attribute("Type").Value) : typeof(string)
                 }).ToList();

            foreach (DataColumn c in columns)
                if (c.DataType == typeof(string))
                {
                    c.ExtendedProperties["ColumnLength"] = 255;
                }
            

            IEnumerable<string> primaryKeys = 
                (from field in fields
                 where field.Attribute("PrimaryKey") != null && field.Attribute("PrimaryKey").Value.ToUpper() == "TRUE"
                 select field.Attribute("Name").Value);

            DataTable dataTable = new DataTable(node.Attribute("Title").Value);
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
                default:
                    return typeof(string);
            }
        }

        private DataTable GetListItemFromAnchor(SyncTableMetadata tableMetadata, SyncSession syncSession, SyncContext syncContext)
        {
            DataTable inserts = Schema.SchemaDataSet.Tables[tableMetadata.TableName].Clone();
            DataTable deletes = Schema.SchemaDataSet.Tables[tableMetadata.TableName].Clone();

            SpWS.Lists listsServices = new SpSync.Data.Server.SpWS.Lists();
            listsServices.Credentials = System.Net.CredentialCache.DefaultCredentials;
            
            string token = 
                tableMetadata.LastReceivedAnchor.Anchor == null ? null : 
                System.Text.ASCIIEncoding.ASCII.GetString(tableMetadata.LastReceivedAnchor.Anchor);

            XElement viewFields = new XElement("Fields");
            foreach (DataColumn column in inserts.Columns)
            {
                var fieldref = new XElement("FieldRef");
                fieldref.SetAttributeValue("Name", column.ColumnName);
                viewFields.Add(fieldref);
            }

            XElement result = 
                listsServices.GetListItemChangesSinceToken(
                    tableMetadata.TableName, 
                    null, 
                    viewFields.GetXmlNode(), 
                    null, 
                    null, 
                    null, 
                    token, 
                    null).GetXElement();

            listsServices.Dispose();


            XNamespace dataNs = result.GetNamespaceOfPrefix("rs");
            XNamespace rowNs  = result.GetNamespaceOfPrefix("z");
            XNamespace defaultNs = result.GetDefaultNamespace();

            XElement changes = result.Element(defaultNs + "Changes");

            string newToken = changes.Attribute("LastChangeToken").Value;
            int MinTimeBetweenSyncs = Int32.Parse(result.Attribute("MinTimeBetweenSyncs").Value);
            int RecommendedTimeBetweenSyncs = Int32.Parse(result.Attribute("RecommendedTimeBetweenSyncs").Value);

            if (changes.Element(defaultNs + "List") != null)
            {
                //Schema has  changed
                // need full resynchronization
            }
            else
            {
                IEnumerable<XElement> changeLog = changes.Elements(defaultNs + "Id");

                foreach (XElement changeItem in changeLog)
                {
                    switch (changeItem.Attribute("ChangeType").Value)
                    { 
                        case "Delete":
                        case "MoveAway":
                            string ID = changeItem.Value;
                            DataRow row = deletes.NewRow();
                            row[deletes.PrimaryKey[0]] = ID;
                            deletes.Rows.Add(row);
                            break;
                        case "InvalidToken":
                        case "Restore":
                            // full resynchronization
                            SyncTracer.Warning("Unknown ChangeType " + changeItem.Attribute("ChangeType").Value);
                            break;
                    }
                }
                deletes.AcceptChanges();
            }

            syncContext.NewAnchor = new SyncAnchor(System.Text.ASCIIEncoding.ASCII.GetBytes(newToken));

            // get added or (updated) rows

            int itemCount = Int32.Parse(result.Element(dataNs + "data").Attribute("ItemCount").Value);
            if (itemCount != 0)
            {
                IEnumerable<XElement> rows = (from row in result.Element(dataNs + "data").Elements(rowNs + "row") select row);

                foreach (var row in rows)
                {
                    IEnumerable<XAttribute> cols =
                        (from col in row.Attributes()
                         where col.Name.LocalName.StartsWith("ows_", true, null)
                         select col);

                    DataRow dataRow = inserts.NewRow();
                    foreach (XAttribute col in cols)
                    {
                        string colName = col.Name.LocalName.Substring("ows_".Length);
                        if (inserts.Columns.Contains(colName))
                            dataRow[colName] = col.Value;
                        else
                            SyncTracer.Warning("Omitting column " + colName);
                    }
                    inserts.Rows.Add(dataRow);
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
    }
}
