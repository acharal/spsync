using System;
using System.Data;
using System.Xml;
using Sp.Data.Caml;
using System.Collections.Generic;

namespace Sp.Data
{
    public class SpConnection : IDbConnection
    {
        private ConnectionState _state;

        private string _connectionString;
       
        private int _connectionTimeout = 1000;

        private string _siteName;

        private string _server;

        private bool isOpen = false;

        private Sp.Data.WS.Lists listService;


        public SpConnection(string server, string site)
        {
            listService = new Sp.Data.WS.Lists();
            _server = server;
            _siteName = site;
        }

        public SpConnection(string connString)
        {
            listService = new Sp.Data.WS.Lists();
            ConnectionString = connString;
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
            _siteName = databaseName;
            ConnectionString = _server + "/" + _siteName + "/" + "_vti_bin/Lists.asmx";
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
                return _connectionString;
            }
            set
            {
                if (isOpen)
                    Close();

                _connectionString = value;
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
            get { return _siteName; }
        }

        public void Open()
        {
            _state = ConnectionState.Open;
            listService.Timeout = this.ConnectionTimeout;
            //listService.Url = _server + "/" + _siteName + "/" + "_vti_bin/Lists.asmx";
            listService.Url = ConnectionString;
            isOpen = true;
        }

        public ConnectionState State
        {
            get { return _state;  }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            listService.Dispose();
        }

        #endregion

        #region Sharepoint specific commands

        public ChangeBatch GetListItemChangesSinceToken(string listName, 
            string viewName, 
            string query, 
            IEnumerable<string> viewFields,
            int rowLimit, 
            QueryOptions queryOptions, 
            string changeToken)
        {
            if (String.IsNullOrEmpty(listName))
                throw new ArgumentNullException("listName");

            XmlDocument viewDoc = new XmlDocument();
            XmlElement viewFieldsNode = viewDoc.CreateElement("ViewFields");
            foreach (string field in viewFields)
            {
                XmlElement fieldRef = viewDoc.CreateElement("FieldRef");
                fieldRef.SetAttribute("Name", field);
                viewFieldsNode.AppendChild(fieldRef);
            }

            XmlDocument queryDoc = new XmlDocument();
            queryDoc.LoadXml(query);

            XmlNode response = listService.GetListItemChangesSinceToken(
                listName,
                viewName,
                queryDoc,
                viewFieldsNode,
                rowLimit.ToString(),
                queryOptions.GetCamlQueryOptions(),
                changeToken,
                null);

            return response.GetXElement().GetCamlChangeBatch();
        }

        public UpdateResults UpdateListItems(string listName, UpdateBatch updateBatch)
        {
            if (String.IsNullOrEmpty(listName))
                throw new ArgumentNullException("listName");

            if (updateBatch.Count == 0)
                throw new ArgumentException("Batch contains no updates", "updateBatch");

            XmlNode response = listService.UpdateListItems(listName, updateBatch.GetCamlUpdateBatch());
            
            return response.GetXElement().GetCamlUpdateResults();
        }

        public ListDef GetListSchema(string listName)
        {
            if (String.IsNullOrEmpty(listName))
                throw new ArgumentNullException("listName");

            XmlNode listDef = listService.GetList(listName);

            return listDef.GetXElement().GetCamlListDef();
        }

        public ListCollection GetLists()
        {
            XmlNode response = listService.GetListCollection();
            return response.GetXElement().GetCamlListCollection();
        }

        #endregion
    }
}
