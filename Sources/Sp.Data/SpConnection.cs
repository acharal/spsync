using System;
using System.Collections.Generic;
using System.Data;

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

        public SpConnection(string server)
        {
            listService = new Sp.Data.WS.Lists();
            _server = server;
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

        void GetListItemChangesSinceToken(string listName, string viewName, string query, int rowLimit, string changeToken)
        {
            listService.GetListItemChangesSinceToken(
                listName,
                viewName,
                null,
                null,
                rowLimit.ToString(),
                null,
                changeToken, 
                null);
        }

        void UpdateListItems(string listName)
        {
            // listService.UpdateListItems();
        
        }
    }
}
