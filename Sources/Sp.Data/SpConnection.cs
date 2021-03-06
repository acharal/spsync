﻿using System;
using System.Data;
using System.Data.Common;
using System.Xml;
using Sp.Data.Caml;
using System.Collections.Generic;
using System.Net;

namespace Sp.Data
{
    public class SpConnection : IDbConnection
    {
        private ConnectionState _state;

        private int _connectionTimeout = 3600000;

        private string _server;

        private ICredentials _credentials = System.Net.CredentialCache.DefaultCredentials; 

        private bool isOpen = false;

        private Sp.Data.WS.Lists listService;

        public ICredentials Credentials
        {    
            get 
            { 
                return _credentials; 
            }
            
            set 
            { 
                _credentials = value; 
            }
        }

        public SpConnection(string connString)
        {
            listService = new Sp.Data.WS.Lists();
            //ConnectionString = connString;
            ParseConnectionString(connString);

        }

        public SpConnection(string server, string username, string password, string domain)
        {
            listService = new Sp.Data.WS.Lists();
            _server = server;

            if (username != null && password != null)
            {
                _credentials = new NetworkCredential(username, password, domain);
            }
        }

        private void ParseConnectionString(string connString)
        {

            _server = connString;

            /*
            if (String.IsNullOrEmpty(connString))
                throw new ArgumentNullException("connString");

            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();

            builder.ConnectionString = connString;

            _server = builder["Data Source"] as string;

            _siteName = builder["Initial Catalog"] as string;

            string username = builder["User ID"] as string;

            string password = builder["Password"] as string;

            if (username != null && password != null)
                _credentials = new NetworkCredential(username, password);
            */
        }

        private string BuildConnectionString()
        {
            throw new NotImplementedException();
        }

        #region IDbConnection Members

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return new SpTransaction(this);
        }

        public IDbTransaction BeginTransaction()
        {
            return new SpTransaction(this);
        }

        public void ChangeDatabase(string databaseName)
        {
            //_siteName = databaseName;
            //ConnectionString = _server + "/" + _siteName + "/" + "_vti_bin/Lists.asmx";
            _server = databaseName;
        }

        public void Close()
        {
            if (_state == ConnectionState.Closed)
                throw new InvalidOperationException("Connection is already closed");

            _state = ConnectionState.Closed;
        }

        public string ConnectionString
        {
            get
            {
                return BuildConnectionString();
            }
            set
            {
                if (value == null)
                    return;

                if (isOpen)
                    Close();

                ParseConnectionString(value);
                Open();
            }
        }

        public int ConnectionTimeout
        {
            get { return _connectionTimeout; }
            set { _connectionTimeout = value; }
        }

        public IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public string Database
        {
            get { return _server; }
        }

        public void Open()
        {
            _state = ConnectionState.Open;
            listService.Timeout = this.ConnectionTimeout;
            //listService.Url = _server + "/" + _siteName + "/" + "_vti_bin/Lists.asmx";
            listService.Url = _server + "/" + "_vti_bin" + "/" + "Lists.asmx";
            listService.Credentials = _credentials;
            isOpen = true;
        }

        public ConnectionState State
        {
            get { return _state; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            listService.Dispose();
        }

        #endregion

        #region Sharepoint specific commands

        public ChangeBatch GetListItemChangesSinceToken(
            string listName, 
            string viewName, 
            string query, 
            IEnumerable<string> viewFields,
            bool splitMetaInfo,
            int rowLimit, 
            QueryOptions queryOptions, 
            string changeToken)
        {
            if (String.IsNullOrEmpty(listName))
                throw new ArgumentNullException("listName");

            XmlDocument viewDoc = new XmlDocument();
            XmlElement viewFieldsNode = viewDoc.CreateElement("ViewFields");
            
            if (splitMetaInfo)
                viewFieldsNode.SetAttribute("Properties", "TRUE");

            foreach (string field in viewFields)
            {
                XmlElement fieldRef = viewDoc.CreateElement("FieldRef");
                fieldRef.SetAttribute("Name", field);
                viewFieldsNode.AppendChild(fieldRef);
            }

            XmlDocument queryDoc = null;
            if (!String.IsNullOrEmpty(query))
            {
                queryDoc = new XmlDocument();
                queryDoc.LoadXml(query);
            }

            XmlNode response = listService.GetListItemChangesSinceToken(
                listName,
                viewName,
                queryDoc,
                viewFieldsNode,
                rowLimit.ToString(),
                queryOptions.GetCamlQueryOptions(),
                changeToken,
                null);

            
            ChangeBatch batch = response.GetXElement().GetCamlChangeBatch();
            
            // catch the case where no changes in list returns the same token
            if (batch.ChangeLog.Count == 0 &&
                batch.NextChangeBatch == changeToken)
            {
                // re-query with empty token in order to get the very latest change token
                // and avoid token expiration
                XmlNode dummyResponse = 
                    listService.GetListItemChangesSinceToken(
                             listName,
                             viewName,
                             queryDoc,
                             viewFieldsNode,
                             "10",
                             queryOptions.GetCamlQueryOptions(),
                             null,
                             null);
                
                ChangeBatch batch2 = response.GetXElement().GetCamlChangeBatch();
                batch.ChangeLog.NextLastChangeToken = batch2.ChangeLog.NextLastChangeToken;
            }

            return batch;
        }

        public ListItemCollection GetListItems(string listName,
            string viewName,
            string query,
            IEnumerable<string> viewFields,
            bool splitMetaInfo,
            int rowLimit,
            QueryOptions queryOptions)
        {

            if (String.IsNullOrEmpty(listName))
                throw new ArgumentNullException("listName");


            XmlDocument viewDoc = new XmlDocument();
            XmlElement viewFieldsNode = viewDoc.CreateElement("ViewFields");

            if (splitMetaInfo)
                viewFieldsNode.SetAttribute("Properties", "TRUE");

            foreach (string field in viewFields)
            {
                XmlElement fieldRef = viewDoc.CreateElement("FieldRef");
                fieldRef.SetAttribute("Name", field);
                viewFieldsNode.AppendChild(fieldRef);
            }

            XmlDocument queryDoc = null;
            if (!String.IsNullOrEmpty(query))
            {
                queryDoc = new XmlDocument();
                queryDoc.LoadXml(query);
            }

            XmlNode response = listService.GetListItems(
                listName,
                viewName,
                queryDoc,
                viewFieldsNode,
                rowLimit.ToString(),
                queryOptions.GetCamlQueryOptions(),
                null);

            return response.GetXElement().GetCamlListItems();
        }

        public UpdateResults UpdateListItems(string listName, UpdateBatch updateBatch)
        {
            if (String.IsNullOrEmpty(listName))
                throw new ArgumentNullException("listName");

            if (updateBatch.Count == 0)
                throw new ArgumentException("Batch contains no updates", "updateBatch");

            // OLD VERSION
            //XmlNode response = listService.UpdateListItems(listName, updateBatch.GetCamlUpdateBatch());
            //return response.GetXElement().GetCamlUpdateResults();

            //NEW
            UpdateResults results = null;
            var batch = updateBatch.GetCamlUpdateBatch(out results);
            if (batch.ChildNodes.Count > 0)
            {
                try
                {
                    XmlNode response = listService.UpdateListItems(listName, batch);
                    var serverResults = response.GetXElement().GetCamlUpdateResults();

                    if (serverResults == null)
                        return results;

                    if (results != null)
                        foreach (var r in results)
                            serverResults.Add(r);

                    return serverResults;
                }
                catch (Exception ex)
                {  //(ex as System.Net.WebException).Status = Timeout , Internalstatus = RequestFatal , Response = null
                //connection exception
                    UpdateResults errorresults = new UpdateResults();

                    foreach (UpdateItem item in updateBatch)
                        errorresults.Add(new UpdateResult()
                        {
                            ErrorCode = UpdateResult.GenericError,
                            ErrorMessage = "Connection Timeout",
                            Command = item.Command,
                            ItemData = item.ChangedItemData,
                            UpdateItemID = item.ID
                        });

                        return errorresults;
                }
            }

            return results;
            //END OF NEW
        }



        public ListDef GetListSchema(string listName)
        {
            if (String.IsNullOrEmpty(listName))
                throw new ArgumentNullException("listName");

            XmlNode listDef = listService.GetList(listName);

            return listDef.GetXElement().GetCamlListDef();
        }

        public ViewDef GetViewSchema(string listName, string viewName)
        {
            if (String.IsNullOrEmpty(listName))
                throw new ArgumentNullException("listName");

            if (String.IsNullOrEmpty(viewName))
                throw new ArgumentNullException("viewName");

            XmlNode viewDef = listService.GetListAndView(listName, viewName);

            return viewDef.GetXElement().GetCamlViewDef();
        }

        public ListCollection GetLists()
        {
            XmlNode response = listService.GetListCollection();
            return response.GetXElement().GetCamlListCollection();
        }

        #endregion
    }
}
